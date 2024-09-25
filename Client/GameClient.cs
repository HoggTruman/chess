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
            string message = await ReadServerMessage();

            HandleServerMessage(message);
        }
    }


    public async Task<string> ReadServerMessage()
    {
        string message = "";
        int bytesRead;

        while ((bytesRead = await _stream.ReadAsync(_buffer, 0, _buffer.Length)) > 0)
        {
            message += Encoding.UTF8.GetString(_buffer, 0, bytesRead);

            if (_stream.DataAvailable == false)
            {
                break;
            }
        }

        return message;
    }


    private async void HandleServerMessage(string message)
    {
        Console.WriteLine(message);
        ServerMessage msgType = (ServerMessage)Char.GetNumericValue(message[0]);

        switch (msgType)
        {
            case ServerMessage.Connected:
                // remove code, get other data, etc ...
                ClientId = Int32.Parse(message.Substring(1));
                break;
        }
    }

}

