using System.Net.Sockets;

namespace Server.Interfaces;

public interface IClient
{
    public int Id { get; }

    public TcpClient TcpClient { get; }
    public NetworkStream Stream { get; }
    public CancellationTokenSource CancellationTokenSource { get; }
    public CancellationToken Token { get; }

    public int RoomId { get; set; }
}
