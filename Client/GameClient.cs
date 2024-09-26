using NetworkShared.Enums;
using System.Net;
using System.Net.Sockets;
using System.Text;
using NetworkShared;

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
            List<byte> message = await ReadServerMessage();

            HandleServerMessage(message);
        }
    }


    public async Task<List<byte>> ReadServerMessage()
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

        return message;
    }


    private async void HandleServerMessage(List<byte> message)
    {
        ServerMessage msgCode = (ServerMessage)MessageHelpers.ReadCode(message);

        switch (msgCode)
        {
            case ServerMessage.Connected:
                // remove code, get other data, etc ...
                break;
        }
    }

}

