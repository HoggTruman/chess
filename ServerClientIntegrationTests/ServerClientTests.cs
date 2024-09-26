using Client;
using NetworkShared;
using NetworkShared.Enums;
using Server;

namespace ServerClientIntegrationTests;

public sealed class ServerClientTests : IDisposable
{
    private GameServer _gameServer;

    public ServerClientTests()
    {
        _gameServer = new();
        _gameServer.Start();
        while (_gameServer.IsListening == false)
        {

        }
    }

    public void Dispose()
    {
        _gameServer.ShutDown();
    }




    [Fact(Timeout=5000)]
    public async void ServerStartsAndShutsDownWithClient()
    {
        GameClient gameClient = new();
        await gameClient.ConnectToServer();
        _gameServer.ShutDown();
    }


    [Fact(Timeout=5000)]
    public async void ClientReceivesConnectedMessage()
    {
        GameClient gameClient = new();
        await gameClient.ConnectToServer();
        List<byte> message = await gameClient.ReadServerMessage();
        ServerMessage msgCode = (ServerMessage)MessageHelpers.ReadCode(message);

        Assert.NotEmpty(message);
        Assert.Equal(ServerMessage.Connected, msgCode);
    }

}
