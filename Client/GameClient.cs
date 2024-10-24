﻿using GameLogic.Enums;
using GameLogic.Interfaces;
using NetworkShared;
using NetworkShared.Enums;
using NetworkShared.Messages.Client;
using NetworkShared.Messages.Server;
using System.Net;
using System.Net.Sockets;

namespace Client;

/// <summary>
/// A class which handles server communication.
/// A new GameClient should be created on hosting a room or on
/// attempting to join a room.
/// </summary>
public class GameClient : IDisposable
{
    #region fields

    private readonly TcpClient _tcpClient;
    private NetworkStream _stream;
    private readonly byte[] _buffer = new byte[16];
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly CancellationToken _token;

    #endregion



    #region Events

    public event Action<int>? RoomHosted;

    public event Action<PieceColor>? StartGame;

    public event Action? RoomNotFound;

    public event Action? RoomFull;

    public event Action<PieceColor>? RoomClosed;

    public event Action<IMove>? MoveReceived;

    #endregion



    public GameClient()
    {
        _tcpClient = new TcpClient();
        _cancellationTokenSource = new CancellationTokenSource();
        _token = _cancellationTokenSource.Token;
    }


    public async Task ConnectToServer()
    {
        var ipEndpoint = new IPEndPoint(IPAddress.Parse(ServerInfo.IpAddress), ServerInfo.Port);
        await _tcpClient.ConnectAsync(ipEndpoint, _token);

        if (_tcpClient.Connected)
        {
            _stream = _tcpClient.GetStream();
        }
    }


    public async Task<byte[]> ReadServerMessage()
    {
        // get message length
        await _stream.ReadAsync(_buffer, 0, 1, _token);
        byte messageLength = _buffer[0];
        
        // read message
        byte[] message = new byte[messageLength];
        message[0] = messageLength;
        await _stream.ReadAsync(message, 1, messageLength - 1, _token);

        return message;
    }


    public void HandleServerMessage(byte[] message)
    {
        ServerMessage msgCode = MessageHelpers.ReadServerCode(message);

        switch (msgCode)
        {
            case ServerMessage.RoomHosted:
                int roomId = RoomHostedMessage.Decode(message);
                RoomHosted?.Invoke(roomId);
                break;

            case ServerMessage.StartGame:
                PieceColor playerColor = StartGameMessage.Decode(message);
                StartGame?.Invoke(playerColor);
                break;

            case ServerMessage.RoomNotFound:
                RoomNotFound?.Invoke();
                break;

            case ServerMessage.RoomFull:
                RoomFull?.Invoke();
                break;

            case ServerMessage.RoomClosed:
                PieceColor winnerColor = RoomClosedMessage.Decode(message);
                RoomClosed?.Invoke(winnerColor);
                break;

            case ServerMessage.Move:
                IMove move = ServerMoveMessage.Decode(message);
                MoveReceived?.Invoke(move);
                break;
        }
    }


    #region Send Message To Server Methods

    /// <summary>
    /// Sends a message to the server to host a room.
    /// </summary>
    /// <param name="hostColor">The PieceColor of the host.</param>
    /// <returns></returns>
    public async Task SendHostRoom(PieceColor hostColor)
    {
        byte[] message = HostRoomMessage.Encode(hostColor);
        await _stream.WriteAsync(message, _token);
    }


    /// <summary>
    /// Sends a message to the server to join a room.
    /// </summary>
    /// <param name="roomId">The ID of the room to join.</param>
    /// <returns></returns>
    public async Task SendJoinRoom(int roomId)
    {
        byte[] message = JoinRoomMessage.Encode(roomId);
        await _stream.WriteAsync(message, _token);
    }


    /// <summary>
    /// Sends a message to the server to cancel hosting a room.
    /// </summary>
    /// <returns></returns>
    public async Task SendCancelHost()
    {
        byte[] message = CancelHostMessage.Encode();
        await _stream.WriteAsync(message, _token);
        _cancellationTokenSource.Cancel();
    }


    /// <summary>
    /// Sends a message to the server containing the player's IMove.
    /// </summary>
    /// <param name="move">The IMove the player is making.</param>
    /// <returns></returns>
    public async Task SendMove(IMove move)
    {
        byte[] message = ClientMoveMessage.Encode(move);
        await _stream.WriteAsync(message, _token);
    }

    #endregion



    public void Dispose()
    {
        _tcpClient.Close();
        _cancellationTokenSource.Dispose();
        GC.SuppressFinalize(this);
    }
}

