using GameLogic.Enums;
using GameLogic.Interfaces;
using NetworkShared;
using NetworkShared.Enums;
using NetworkShared.Messages.Client;
using NetworkShared.Messages.Server;
using NetworkShared.Messages.Shared;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace Server;

public class GameServer
{
    public Dictionary<int, Client> Clients = [];
    public Dictionary<int, Room> Rooms = [];

    private readonly TcpListener _tcpListener;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly CancellationToken _token;


    public GameServer(TcpListener tcpListener)
    {
        _tcpListener = tcpListener;
        _token = _cancellationTokenSource.Token;
    }


    public void Start()
    {
        _tcpListener.Start();
        Console.WriteLine("Server is listening for connections...");
        Task.Run(AcceptClients); // exceptions lost??
    }


    private async Task AcceptClients()
    {
        while (_token.IsCancellationRequested == false)
        {
            TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync(_token);

            Client client = new(
                tcpClient,
                new CancellationTokenSource(),
                StartClientCommunications);

            Clients[client.Id] = client;

            Console.WriteLine($"[{DateTime.Now}] Client connected with IP {tcpClient.Client.RemoteEndPoint}");
        }
    }



    /// <summary>
    /// Disconnects all clients and shuts down the server.
    /// </summary>
    /// <returns></returns>
    public async Task ShutDown()
    {
        if (_token.IsCancellationRequested)
        {
            // prevent attempting to shutdown twice
            return;
        }

        // Stops the server accepting new clients
        _cancellationTokenSource.Cancel();        

        // cancel each client's token
        ConcurrentBag<Task> clientTasks = [];

        foreach (Client client in Clients.Values)
        {
            client.CancellationTokenSource.Cancel();
            clientTasks.Add(client.ServerCommunications);
        }

        // Check the exceptions from the clients are all OperationCancelledException
        Task resolvePlayerTasks = Task.WhenAll(clientTasks);

        try
        {
            await resolvePlayerTasks;
        }
        catch (Exception)
        {
            if (resolvePlayerTasks.Exception != null)
            {
                var exceptions = resolvePlayerTasks.Exception.Flatten().InnerExceptions;

                foreach(Exception e in exceptions)
                {
                    if (e is not OperationCanceledException)
                    {
                        Console.WriteLine(e.Message);
                    }
                }   
            }         
        }

        _tcpListener.Stop();
        _tcpListener.Dispose();
        _cancellationTokenSource.Dispose();

        foreach (var client in Clients.Values)
        {
            client.TcpClient.Close();
            client.CancellationTokenSource.Dispose();
        }
    }


    private async Task StartClientCommunications(Client client)
    {
        while (client.Token.IsCancellationRequested == false && client.TcpClient.Connected)
        {
            byte[] message = await ReadClientMessage(client.Stream, client.Token);
            await HandleClientMessage(client, message);
        }
    }


    private async Task<byte[]> ReadClientMessage(NetworkStream stream, CancellationToken clientToken)
    {
        // get message length
        byte[] buffer = new byte[1];
        await stream.ReadAsync(buffer, 0, 1, clientToken);
        byte messageLength = buffer[0];
        
        // read message
        byte[] message = new byte[messageLength];
        message[0] = messageLength;
        await stream.ReadAsync(message, 1, messageLength - 1, clientToken);

        return message;
    }


    private async Task HandleClientMessage(Client client, byte[] inMsg)
    {
        ClientMessage msgCode = MessageHelpers.ReadClientCode(inMsg);

        switch (msgCode)
        {
            case ClientMessage.HostRoom:
                PieceColor hostColor = HostRoomMessage.Decode(inMsg);
                HandleHostRoom(client, hostColor);
                await RespondHostRoom(client);
                break;

            case ClientMessage.JoinRoom:
                int roomId = JoinRoomMessage.Decode(inMsg);
                ServerMessage response = HandleJoinRoom(client, roomId);
                await RespondJoinRoom(client, response);
                break;

            case ClientMessage.CancelHost:
                await CloseRoom(client.RoomId, PieceColor.None);
                break;

            case ClientMessage.Move:
                IMove move = MoveMessage.Decode(inMsg);
                bool isValidMove = HandleMove(client, move);
                await RespondMove(client, isValidMove, inMsg);
                break;

            default:
                // Disconnect Player
                break;
        }
    }
    

    /// <summary>
    /// Closes a room and disconnects its clients.
    /// A RoomClosed message is sent to the clients containing the winnerColor.
    /// </summary>
    /// <param name="roomId">The Id of the room to close.</param>
    /// <param name="winnerColor">The PieceColor of the winner to send to the clients.</param>
    /// <returns></returns>
    private async Task CloseRoom(int roomId, PieceColor winnerColor)
    {
        Room room = Rooms[roomId];
        Rooms.Remove(roomId);

        ConcurrentBag<Task> playerTasks = [];

        foreach(Client client in room.Players)
        {
            Clients.Remove(client.Id);

            if (client.Stream.CanWrite)
            {
                await client.Stream.WriteAsync(RoomClosedMessage.Encode(winnerColor), _token);
            }
            
            client.CancellationTokenSource.Cancel();
            playerTasks.Add(client.ServerCommunications);
        }

        // Check the exceptions from the clients are all OperationCancelledException
        Task resolvePlayerTasks = Task.WhenAll(playerTasks);

        try
        {
            await resolvePlayerTasks;
        }
        catch (Exception)
        {
            if (resolvePlayerTasks.Exception != null)
            {
                var exceptions = resolvePlayerTasks.Exception.Flatten().InnerExceptions;

                foreach(Exception e in exceptions)
                {
                    if (e is not OperationCanceledException)
                    {
                        Console.WriteLine(e.Message);
                    }
                }   
            }
        }

        foreach(Client client in room.Players)
        {
            client.TcpClient.Close();
            client.CancellationTokenSource.Dispose();
        }
    }



    #region Message Handlers

    /// <summary>
    /// Creates a Room with the Client as the specified PieceColor.
    /// </summary>
    /// <param name="client">The Client object of the hosting player.</param>
    /// <param name="hostColor">The PieceColor of the hosting player.</param>
    public void HandleHostRoom(Client client, PieceColor hostColor)
    {
        Room room = new(client, hostColor);

        Rooms[room.Id] = room;
        
        client.RoomId = room.Id;
    }


    /// <summary>
    /// Attempts to add the Client to the Room with roomId.
    /// </summary>
    /// <param name="client">The Client object for the joining player.</param>
    /// <param name="roomId">The Id of the room to join.</param>
    /// <returns>A ServerMessage enum corresponding to the response.</returns>
    public ServerMessage HandleJoinRoom(Client client, int roomId)
    {
        if (Rooms.TryGetValue(roomId, out Room? room) == false ||
            room == null)
        {
            return ServerMessage.RoomNotFound;
        }

        if (room.TryJoin(client))
        {
            client.RoomId = room.Id;
            return ServerMessage.StartGame; 
        }
        
        return ServerMessage.RoomFull;
    }


    /// <summary>
    /// Attempts to apply the move to the client's room.
    /// </summary>
    /// <param name="client">The Client object for the moving player.</param>
    /// <param name="move">The IMove to attempt.</param>
    /// <returns>true if the move is valid and applied successfully. Otherwise, false.</returns>
    public bool HandleMove(Client client, IMove move)
    {
        Room room = Rooms[client.RoomId];
        return room.TryMove(client, move);        
    }

    #endregion



    #region Response Methods

    /// <summary>
    /// Sends a RoomHostedMessage back to the Client.
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    private async Task RespondHostRoom(Client client)
    {
        byte[] outMsg = RoomHostedMessage.Encode(client.RoomId);
        await client.Stream.WriteAsync(outMsg, client.Token);
    }


    /// <summary>
    /// Sends a StartGameMessage to the host and joiner if the Client could join the room.
    /// Otherwise, sends a RoomNotFoundMessage or RoomFullMessage to the Client.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="response"></param>
    /// <returns></returns>
    private async Task RespondJoinRoom(Client client, ServerMessage response)
    {
        if (response == ServerMessage.StartGame)
        {
            Room room = Rooms[client.RoomId];

            // send message to joiner
            PieceColor joinerColor = room.PlayerColors[client];
            byte[] joinerMessage = StartGameMessage.Encode(joinerColor);
            await client.Stream.WriteAsync(joinerMessage, client.Token);

            // send message to host
            Client host = room.Host;
            byte[] hostMessage = StartGameMessage.Encode(room.PlayerColors[host]);
            await host.Stream.WriteAsync(hostMessage, host.Token);
        }
        else if (response == ServerMessage.RoomNotFound)
        {
            byte[] joinerMessage = RoomNotFoundMessage.Encode();
            await client.Stream.WriteAsync(joinerMessage, client.Token);
        }
        else if (response == ServerMessage.RoomFull)
        {
            byte[] joinerMessage = RoomFullMessage.Encode();
            await client.Stream.WriteAsync(joinerMessage, client.Token);
        }
    }


    /// <summary>
    /// Relays the move to the client's opponent if it is valid.
    /// Otherwise closes the room, with the opponent as the winner.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="isValidMove"></param>
    /// <param name="moveMessage"></param>
    /// <returns></returns>
    private async Task RespondMove(Client client, bool isValidMove, byte[] moveMessage)
    {
        Room room = Rooms[client.RoomId];
        Client opponent = room.GetOpponent(client);

        if (isValidMove)
        {
            await opponent.Stream.WriteAsync(moveMessage, opponent.Token);

            if (room.GameIsOver())
            {
                await CloseRoom(client.RoomId, room.GetWinner());
            }
        }
        else
        {            
            PieceColor winnerColor = room.PlayerColors[opponent];
            await CloseRoom(client.RoomId, winnerColor);
        }
    }

    #endregion


}

