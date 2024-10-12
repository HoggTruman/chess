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
    public bool IsListening { get; private set; }
    public Dictionary<int, Client> Clients = [];
    public Dictionary<int, Room> Rooms = [];

    private readonly TcpListener _tcpListener;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly CancellationToken _token;
    private readonly ConcurrentBag<Task> _clientTasks = [];



    public GameServer()
    {
        var ipEndPoint = new IPEndPoint(IPAddress.Parse(ServerInfo.IpAddress), ServerInfo.Port);
        _tcpListener = new TcpListener(ipEndPoint);
        _token = _cancellationTokenSource.Token;
    }


    public async Task Start()
    {
        _tcpListener.Start();
        IsListening = true;

        while (_token.IsCancellationRequested == false)
        {
            Console.WriteLine("Server is listening for connections...");
            TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync(_token);

            Client client = new(tcpClient);
            Clients[client.Id] = client;

            Console.WriteLine($"[{DateTime.Now}] Client connected with IP {tcpClient.Client.RemoteEndPoint}");

            _clientTasks.Add(StartClientCommunications(client));
        }
    }


    public async Task ShutDown()
    {
        if (_token.IsCancellationRequested)
        {
            // prevent attempting to shutdown twice
            return;
        }

        _cancellationTokenSource.Cancel();

        try
        {
            await Task.WhenAll(_clientTasks);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"{nameof(OperationCanceledException)} thrown");
        }

        _tcpListener.Stop();
        IsListening = false;

        _tcpListener.Dispose();
        _cancellationTokenSource.Dispose();

        foreach (var client in Clients.Values)
        {
            client.TcpClient.Close();
        }
    }


    private async Task StartClientCommunications(Client client)
    {
        while (client.TcpClient.Connected && _token.IsCancellationRequested == false)
        {
            byte[] message = await ReadClientMessage(client.Stream);
            await HandleClientMessage(client, message);
        }
    }


    private async Task<byte[]> ReadClientMessage(NetworkStream stream)
    {
        byte[] buffer = new byte[256];
        
        List<byte> message = [];
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer, _token)) > 0)
        {
            for (int i = 0; i < bytesRead; i++)
            {
                message.Add(buffer[i]);
            }

            if (stream.DataAvailable == false)
            {
                break;
            }
        }

        return message.ToArray();
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
                await ShutDownRoom(client.RoomId, PieceColor.None);
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
    


    private async Task ShutDownRoom(int roomId, PieceColor winnerColor)
    {
        Room room = Rooms[roomId];
        Rooms.Remove(roomId);

        foreach(Client client in room.Players)
        {
            if (client.Stream.CanWrite)
            {
                await client.Stream.WriteAsync(RoomClosedMessage.Encode(winnerColor), _token);
            }
            
            client.TcpClient.Close();
            Clients.Remove(client.Id);
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
        await client.Stream.WriteAsync(outMsg, _token);
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
            await client.Stream.WriteAsync(joinerMessage, _token);

            // send message to host
            Client host = room.Host;
            byte[] hostMessage = StartGameMessage.Encode(room.PlayerColors[host]);
            await host.Stream.WriteAsync(hostMessage, _token);
        }
        else if (response == ServerMessage.RoomNotFound)
        {
            byte[] joinerMessage = RoomNotFoundMessage.Encode();
            await client.Stream.WriteAsync(joinerMessage, _token);
        }
        else if (response == ServerMessage.RoomFull)
        {
            byte[] joinerMessage = RoomFullMessage.Encode();
            await client.Stream.WriteAsync(joinerMessage, _token);
        }
    }


    /// <summary>
    /// Relays the move to the client's opponent if it is valid.
    /// Otherwise shuts down the room, with the opponent as the winner.
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
            await opponent.Stream.WriteAsync(moveMessage, _token);
        }
        else
        {            
            PieceColor winnerColor = room.PlayerColors[opponent];
            await ShutDownRoom(client.RoomId, winnerColor);
        }
    }

    #endregion


}

