using System.Net.Sockets;

namespace Server;

public class Client
{
    private static int _count = 0;

    public int Id { get; }

    public TcpClient TcpClient { get; }
    public NetworkStream Stream { get; }

    public int OpponentId { get; set; }
    public int RoomId { get; set; }


    public Client(TcpClient tcpClient)
    {
        Id = ++_count;
        TcpClient = tcpClient;
        Stream = tcpClient.GetStream();
    }
}

