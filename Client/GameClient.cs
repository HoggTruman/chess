using GameLogic.Enums;
using GameLogic.Interfaces;
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
    private readonly byte[] _buffer = new byte[16];


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
        // get message length
        await _stream.ReadAsync(_buffer, 0, 1);
        byte messageLength = _buffer[0];
        
        // read message
        byte[] message = new byte[messageLength];
        message[0] = messageLength;
        await _stream.ReadAsync(message, 1, messageLength - 1);

        return message;
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
            case ServerMessage.Move:
                //
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
        await _stream.WriteAsync(message);
    }


    /// <summary>
    /// Sends a message to the server to join a room.
    /// </summary>
    /// <param name="roomId">The ID of the room to join.</param>
    /// <returns></returns>
    public async Task SendJoinRoom(int roomId)
    {
        byte[] message = JoinRoomMessage.Encode(roomId);
        await _stream.WriteAsync(message);
    }


    /// <summary>
    /// Sends a message to the server to cancel hosting a room.
    /// </summary>
    /// <returns></returns>
    public async Task SendCancelHost()
    {
        byte[] message = CancelHostMessage.Encode();
        await _stream.WriteAsync(message);
        _tcpClient.Close(); // make a new GameClient every time a client hosts
    }


    /// <summary>
    /// Sends a message to the server containing the player's IMove.
    /// </summary>
    /// <param name="move">The IMove the player is making.</param>
    /// <returns></returns>
    public async Task SendMove(IMove move)
    {
        byte[] message = ClientMoveMessage.Encode(move);
        await _stream.WriteAsync(message);
    }

    #endregion
}

