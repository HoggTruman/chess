using GameLogic.Enums;
using NetworkShared;
using NetworkShared.Enums;
using NetworkShared.Messages.Client;
using System.Net;
using System.Net.Sockets;

namespace Client;

public class GameClient
{
    public TcpClient _tcpClient { get; private set; }
    private NetworkStream _stream;
    private byte[] _buffer = new byte[1024];

    public int ClientId;


    public GameClient()
    {
        _tcpClient = new TcpClient();
    }


    public async Task ConnectToServer()
    {
        var ipEndpoint = new IPEndPoint(IPAddress.Parse(ServerInfo.ipAddress), ServerInfo.port);
        await _tcpClient.ConnectAsync(ipEndpoint);

        if (_tcpClient.Connected)
        {
            _stream = _tcpClient.GetStream();
        }
    }


    private async Task StartServerCommunications()
    {
        while (_tcpClient.Connected)
        {
            byte[] message = await ReadServerMessage();

            HandleServerMessage(message);
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
        ServerMessage msgCode = (ServerMessage)MessageHelpers.ReadCode(message);

        switch (msgCode)
        {
            case ServerMessage.Connected:
                // remove code, get other data, etc ...
                break;
            case ServerMessage.RoomHosted:
                // trigger an event?
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
}

