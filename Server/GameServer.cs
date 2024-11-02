using GameLogic.Enums;
using GameLogic.Interfaces;
using NetworkShared;
using NetworkShared.Enums;
using NetworkShared.Messages.Client;
using NetworkShared.Messages.Server;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Server;

public class GameServer
{
    public Dictionary<int, Client> Clients = [];
    public Dictionary<int, Room> Rooms = [];
    private readonly Dictionary<Client, Task> _clientTasks = [];

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
        _ = AcceptClients();
    }


    private async Task AcceptClients()
    {
        while (_token.IsCancellationRequested == false)
        {
            try
            {
                TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync(_token);

                if (tcpClient.Connected)
                {
                    Console.WriteLine($"[{DateTime.Now}] Client connected with IP {tcpClient.Client.RemoteEndPoint}");
                    Client client = new(tcpClient, new CancellationTokenSource());
                    _clientTasks[client] = Task.Run(() => StartClientCommunications(client));
                    Clients[client.Id] = client;                
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"[{DateTime.Now}] SocketException: {ex}");
            }
            catch (OperationCanceledException)
            {
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}] {ex}");
            }
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

        Console.WriteLine($"[{DateTime.Now}] Server shutting down");

        // Stops the server accepting new clients
        _cancellationTokenSource.Cancel();        

        foreach (Client client in Clients.Values)
        {
            try
            {
                client.CancellationTokenSource.Cancel();
            }
            catch (ObjectDisposedException)
            {

            }
        }

        ConcurrentBag<Task> remTasks = [.._clientTasks.Values];
        await Task.WhenAll(remTasks);

        _tcpListener.Stop();
        _cancellationTokenSource.Dispose();
    }


    private async Task StartClientCommunications(Client client)
    {
        while (client.TcpClient.Connected &&
               client.Token.IsCancellationRequested == false)
        {
            try
            {
                byte[] message = await ReadClientMessage(client.Stream, client.Token);
                if (message.Length != 0)
                {
                    await HandleClientMessage(client, message);
                }                
            }
            catch (OperationCanceledException)
            {
                // OperationCanceledException means client is disconnected by the server.
            }
            catch (IOException)
            {
                Console.WriteLine($"[{DateTime.Now}] Lost connection to client with IP {client.TcpClient.Client.RemoteEndPoint}");
                Room room = Rooms[client.RoomId];
                PieceColor winnerColor = room.GetOpponentColor(client);
                await CloseRoom(client.RoomId, winnerColor); 
            }
        }        
        
        Console.WriteLine($"[{DateTime.Now}] Disconnected client with IP {client.TcpClient.Client.RemoteEndPoint}");
        Clients.Remove(client.Id);
        _clientTasks.Remove(client);        
        client.TcpClient.Close();
        client.CancellationTokenSource.Dispose();
    }


    /// <summary>
    /// Reads a message from the provided stream.
    /// </summary>
    /// <param name="stream">The NetworkStream to read the message from</param>
    /// <param name="clientToken">The client's CancellationToken</param>
    /// <returns>
    /// A Task that represents the asynchronous read operation. 
    /// The value of its result is the message read as a byte array.
    /// </returns>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="IOException"></exception>
    private async Task<byte[]> ReadClientMessage(NetworkStream stream, CancellationToken clientToken)
    {
        // get message length
        byte[] buffer = new byte[1];
        int bytesRead = await stream.ReadAsync(buffer, 0, 1, clientToken);
        
        if (bytesRead == 0)
        {
            return [];
        }        
        
        // read message
        byte messageLength = buffer[0];
        byte[] message = new byte[messageLength];
        message[0] = messageLength;
        await stream.ReadAsync(message, 1, messageLength - 1, clientToken);

        return message;
    }


    /// <summary>
    /// Handles the server response to the client's message.
    /// </summary>
    /// <param name="client">The client the message was received from.</param>
    /// <param name="inMsg">The message received from the client.</param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    private async Task HandleClientMessage(Client client, byte[] inMsg)
    {
        client.Token.ThrowIfCancellationRequested();

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
                IMove move = ClientMoveMessage.Decode(inMsg);
                bool isValidMove = HandleMove(client, move);
                await RespondMove(client, isValidMove, move);
                break;

            default:
                HandleUnknownMessageType(client);
                break;
        }
    }
    

    /// <summary>
    /// Closes a room and cancels the CancellationToken of each Client in the room.
    /// A RoomClosed message is sent to the clients containing the winnerColor.
    /// </summary>
    /// <param name="roomId">The Id of the room to close.</param>
    /// <param name="winnerColor">The PieceColor of the winner to send to the clients.</param>
    /// <returns></returns>
    private async Task CloseRoom(int roomId, PieceColor winnerColor)
    {
        Room room = Rooms[roomId];
        Rooms.Remove(roomId);

        foreach(Client client in room.Players)
        {
            try
            {
                client.CancellationTokenSource.Cancel();

                if (client.TcpClient.Connected)
                {
                    await client.Stream.WriteAsync(RoomClosedMessage.Encode(winnerColor));
                }
            }
            catch (ObjectDisposedException)
            {

            }
            catch (IOException)
            {
                Console.WriteLine($"[{DateTime.Now}] Lost connection to client with IP {client.TcpClient.Client.RemoteEndPoint}");
            }
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


    /// <summary>
    /// Disconnects the client and shuts down their room when an unrecognised message is received.
    /// </summary>
    /// <param name="client">The Client object of the client to disconnect.</param>
    public async void HandleUnknownMessageType(Client client)
    {
        Console.WriteLine($"Invalid Message received from client with IP {client.TcpClient.Client.RemoteEndPoint}");

        if (Rooms.TryGetValue(client.RoomId, out Room? room) &&
            room != null)
        {
            PieceColor winnerColor = room.GetOpponentColor(client);
            await CloseRoom(client.RoomId, winnerColor);
        }
        else
        {
            client.CancellationTokenSource.Cancel();
        }
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
        try
        {
            await client.Stream.WriteAsync(outMsg, client.Token);
        }
        catch (IOException)
        {
            Console.WriteLine($"[{DateTime.Now}] Lost connection to client with IP {client.TcpClient.Client.RemoteEndPoint}");
            await CloseRoom(client.RoomId, PieceColor.None);            
        }        
    }


    /// <summary>
    /// Sends a StartGameMessage to the host and joiner if the Client could join the room.
    /// Otherwise, sends a RoomNotFoundMessage or RoomFullMessage to the Client.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="response"></param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    private async Task RespondJoinRoom(Client client, ServerMessage response)
    {
        client.Token.ThrowIfCancellationRequested();

        if (response == ServerMessage.StartGame)
        {
            // Try send message to joiner
            Room room = Rooms[client.RoomId];
            PieceColor joinerColor = room.PlayerColors[client];
            byte[] joinerMessage = StartGameMessage.Encode(joinerColor);
            try
            {                
                await client.Stream.WriteAsync(joinerMessage, client.Token);
            }
            catch (IOException)
            {
                Console.WriteLine($"[{DateTime.Now}] Lost connection to client with IP {client.TcpClient.Client.RemoteEndPoint}");
                await CloseRoom(client.RoomId, PieceColor.None);
            }

            // Try send message to host
            Client host = room.Host;
            byte[] hostMessage = StartGameMessage.Encode(room.PlayerColors[host]);
            try
            {                
                await host.Stream.WriteAsync(hostMessage, host.Token);
            }
            catch (IOException)
            {
                Console.WriteLine($"[{DateTime.Now}] Lost connection to client with IP {host.TcpClient.Client.RemoteEndPoint}");
            }
        }
        else if (response == ServerMessage.RoomNotFound)
        {            
            try
            {
                byte[] joinerMessage = RoomNotFoundMessage.Encode();
                await client.Stream.WriteAsync(joinerMessage, client.Token);
                client.CancellationTokenSource.Cancel();
            }
            catch (IOException)
            {
                Console.WriteLine($"[{DateTime.Now}] Lost connection to client with IP {client.TcpClient.Client.RemoteEndPoint}");
            }
        }
        else if (response == ServerMessage.RoomFull)
        {            
            try
            {
                byte[] joinerMessage = RoomFullMessage.Encode();
                await client.Stream.WriteAsync(joinerMessage, client.Token);
                client.CancellationTokenSource.Cancel();
            }
            catch
            {
                Console.WriteLine($"[{DateTime.Now}] Lost connection to client with IP {client.TcpClient.Client.RemoteEndPoint}");
            }            
        }
    }


    /// <summary>
    /// Relays the move to the client's opponent if it is valid.
    /// Otherwise closes the room, with the opponent as the winner.
    /// </summary>
    /// <param name="client">The Client of the player who made the move.</param>
    /// <param name="isValidMove">A bool of whether the move is valid.</param>
    /// <param name="move">The move to relay to the opponent.</param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    private async Task RespondMove(Client client, bool isValidMove, IMove move)
    {
        client.Token.ThrowIfCancellationRequested();

        Room room = Rooms[client.RoomId];
        Client opponent = room.GetOpponent(client);

        if (isValidMove)
        {
            try
            {
                byte[] message = ServerMoveMessage.Encode(move);
                await opponent.Stream.WriteAsync(message, opponent.Token);
            }
            catch (IOException)
            {
                await CloseRoom(client.RoomId, room.PlayerColors[client]);
                return;
            }

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

