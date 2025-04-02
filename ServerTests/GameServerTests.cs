using FluentAssertions;
using NetworkShared;
using Server;
using System.Net;
using System.Net.Sockets;

namespace ServerTests;

public class GameServerTests
{
    private readonly TcpListener _tcpListener;

    public GameServerTests()
    {
        IPEndPoint ipEndPoint = new(IPAddress.Parse(ServerInfo.IpAddress), ServerInfo.Port);
        _tcpListener = new(ipEndPoint);
    }

    

    [Fact]
    public async Task ServerStartsAndShutsDown()
    {
        // Arrange 
        GameServer server = new(_tcpListener);

        // Act
        server.Start();
        await server.ShutDown();
    }
}
