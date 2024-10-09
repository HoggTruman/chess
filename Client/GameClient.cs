﻿using GameLogic.Enums;
using NetworkShared;
using NetworkShared.Enums;
using NetworkShared.Messages.Client;
using System.Net;
using System.Net.Sockets;

namespace Client;

/// <summary>
/// A class which handles server communication.
/// A new GameClient should be created on hosting a room or on
/// attempting to join a room.
/// </summary>
public class GameClient
{
    public TcpClient _tcpClient { get; private set; }
    private NetworkStream _stream;
    private byte[] _buffer = new byte[1024];


    public GameClient()
    {
        _tcpClient = new TcpClient();
    }


    public async Task ConnectToServer()
    {
        var ipEndpoint = new IPEndPoint(IPAddress.Parse(ServerInfo.IpAddress), ServerInfo.Port);
        await _tcpClient.ConnectAsync(ipEndpoint);

        if (_tcpClient.Connected)
        {
            _stream = _tcpClient.GetStream();
        }
    }


    public async Task<byte[]> ReadServerMessage()
    {
        List<byte> message = [];
        int bytesRead;

        while ((bytesRead = await _stream.ReadAsync(_buffer, 0, _buffer.Length)) > 0)
        {
            for (int i = 0; i < bytesRead; i++)
            {
                message.Add(_buffer[i]);
            }

            if (_stream.DataAvailable == false)
            {
                break;
            }
        }

        return message.ToArray();
    }


    private async void HandleServerMessage(byte[] message)
    {
        ServerMessage msgCode = MessageHelpers.ReadServerCode(message);

        switch (msgCode)
        {
            case ServerMessage.RoomHosted:
                // trigger an event?
                break;
            case ServerMessage.StartGame:
                //
                break;
            case ServerMessage.RoomClosed:
                //
                break;
        }
    }


    public async Task HostRoom(PieceColor hostColor)
    {
        byte[] message = HostRoomMessage.Encode(hostColor);
        await _stream.WriteAsync(message);
    }


    public async Task JoinRoom(int roomId)
    {
        byte[] message = JoinRoomMessage.Encode(roomId);
        await _stream.WriteAsync(message);
    }


    public async Task CancelHost()
    {
        byte[] message = CancelHostMessage.Encode();
        await _stream.WriteAsync(message);
        _tcpClient.Close(); // make a new GameClient every time a client hosts
    }
}

