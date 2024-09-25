using Client;
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
        string result = await gameClient.ReadServerMessage();

        Assert.NotEmpty(result);
    }

}
