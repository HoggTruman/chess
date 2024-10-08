using Client;
using GameLogic.Enums;
using GameLogic.Helpers;
using NetworkShared;
using NetworkShared.Enums;
using NetworkShared.Messages.Server;
using Server;

namespace ServerClientIntegrationTests;

public sealed class ServerClientTests : IAsyncLifetime
{
    private GameServer _gameServer;

    public ServerClientTests()
    {
        _gameServer = new();
        Task serverStart = _gameServer.Start();
        while (_gameServer.IsListening == false)
        {
            // wait for game server to listen
        }
    }


    public async Task InitializeAsync()
    {
        //await Task.Delay(1000);
    }


    public async Task DisposeAsync()
    {
        await _gameServer.ShutDown();
    }




    [Fact(Timeout=5000)]
    public async void ServerStartsAndShutsDownWithClient()
    {
        GameClient gameClient = new();
        await gameClient.ConnectToServer();
        await _gameServer.ShutDown();
    }


    [Fact(Timeout=5000)]
    public async void ClientReceivesConnectedMessage()
    {
        GameClient gameClient = new();
        await gameClient.ConnectToServer();
        byte[] message = await gameClient.ReadServerMessage();
        ServerMessage msgCode = (ServerMessage)MessageHelpers.ReadCode(message);

        Assert.NotEmpty(message);
        Assert.Equal(ServerMessage.Connected, msgCode);
    }


    [Fact(Timeout=5000)]
    public async void ClientSendsHostRoom_ReceivesRoomHosted()
    {
        GameClient gameClient = new();
        await gameClient.ConnectToServer();
        byte[] connectedMessage = await gameClient.ReadServerMessage();

        await gameClient.HostRoom(PieceColor.White);
        byte[] roomHostedMessage = await gameClient.ReadServerMessage();
        int roomId = RoomHostedMessage.Decode(roomHostedMessage);

        Assert.True(roomId > 0);

    }


    [Fact(Timeout=5000)]
    public async void ClientSendsJoinRoom_WithNoHostedRooms_ReceivesRoomNotFound()
    {
        GameClient gameClient = new();
        await gameClient.ConnectToServer();
        byte[] connectedMessage = await gameClient.ReadServerMessage();

        int roomId = 1234;
        await gameClient.JoinRoom(roomId);
        byte[] roomNotFoundMessage = await gameClient.ReadServerMessage();
        ServerMessage responseCode = (ServerMessage)MessageHelpers.ReadCode(roomNotFoundMessage);

        Assert.NotEmpty(roomNotFoundMessage);
        Assert.Equal(ServerMessage.RoomNotFound, responseCode);
    }


    [Fact(Timeout=5000)]
    public async void ClientSendsJoinRoom_WithRoomAlreadyFull_ReceivesRoomFull()
    {
        // Connect Clients
        GameClient hostClient = new();
        GameClient joiningClient = new();
        GameClient testClient = new();

        await hostClient.ConnectToServer();
        await hostClient.ReadServerMessage();
        await joiningClient.ConnectToServer();
        await joiningClient.ReadServerMessage();
        await testClient.ConnectToServer();
        await testClient.ReadServerMessage();

        // Host Room
        await hostClient.HostRoom(PieceColor.White);
        byte[] roomHostedMessage = await hostClient.ReadServerMessage();
        int roomId = RoomHostedMessage.Decode(roomHostedMessage);

        // Join Room
        await joiningClient.JoinRoom(roomId);
        byte[] joinerResponseMessage = await joiningClient.ReadServerMessage();

        // Attempt to join the full room
        await testClient.JoinRoom(roomId);
        byte[] testResponseMessage = await testClient.ReadServerMessage();
        ServerMessage responseCode = (ServerMessage)MessageHelpers.ReadCode(testResponseMessage);

        Assert.NotEmpty(testResponseMessage);
        Assert.Equal(ServerMessage.RoomFull, responseCode);
    }


    [Fact(Timeout=5000)]
    public async void ClientJoinsRoom_HostAndJoinerReceiveStartGame()
    {
        // Connect Clients
        GameClient hostClient = new();
        GameClient joiningClient = new();

        await hostClient.ConnectToServer();
        await hostClient.ReadServerMessage();
        await joiningClient.ConnectToServer();
        await joiningClient.ReadServerMessage();

        // Host Room
        PieceColor hostColor = PieceColor.White;
        await hostClient.HostRoom(hostColor);
        byte[] roomHostedMessage = await hostClient.ReadServerMessage();
        int roomId = RoomHostedMessage.Decode(roomHostedMessage);

        // Join Room
        await joiningClient.JoinRoom(roomId);
        byte[] joinerResponse = await joiningClient.ReadServerMessage();
        ServerMessage joinerResponseCode = (ServerMessage)MessageHelpers.ReadCode(joinerResponse);

        Assert.NotEmpty(joinerResponse);
        Assert.Equal(ServerMessage.StartGame, joinerResponseCode);

        PieceColor joinerResponseColor = StartGameMessage.Decode(joinerResponse);
        Assert.Equal(ColorHelpers.Opposite(hostColor), joinerResponseColor);

        // Host Response
        byte[] hostResponse = await hostClient.ReadServerMessage();        
        ServerMessage hostResponseCode = (ServerMessage)MessageHelpers.ReadCode(hostResponse);       
        
        Assert.NotEmpty(hostResponse);
        Assert.Equal(ServerMessage.StartGame, hostResponseCode);

        PieceColor hostResponseColor = StartGameMessage.Decode(hostResponse);
        Assert.Equal(ColorHelpers.Opposite(hostColor), joinerResponseColor);

    }


    [Fact(Timeout=5000)]
    public async void RoomCancelled_BeforeJoined_JoinerGetsRoomNotFound()
    {
        // Connect Host
        GameClient hostClient = new();
        await hostClient.ConnectToServer();
        await hostClient.ReadServerMessage();

        // Host Room
        PieceColor hostColor = PieceColor.White;
        await hostClient.HostRoom(hostColor);
        byte[] roomHostedMessage = await hostClient.ReadServerMessage();
        int roomId = RoomHostedMessage.Decode(roomHostedMessage);

        // Cancel Host
        await hostClient.CancelHost();
        await Task.Delay(10); // Make sure the room is fully closed down

        // Connect Joiner
        GameClient joiningClient = new();
        await joiningClient.ConnectToServer();
        await joiningClient.ReadServerMessage();

        // Attempt to Join Room
        await joiningClient.JoinRoom(roomId);
        byte[] joinerResponse = await joiningClient.ReadServerMessage();
        ServerMessage joinerResponseCode = (ServerMessage)MessageHelpers.ReadCode(joinerResponse);

        Assert.NotEmpty(joinerResponse);
        Assert.Equal(ServerMessage.RoomNotFound, joinerResponseCode);
    }


    [Fact(Timeout=5000)]
    public async void RoomCancelled_AfterJoined_JoinerGetsStartGame()
    {
        // Connect Host
        GameClient hostClient = new();
        await hostClient.ConnectToServer();
        await hostClient.ReadServerMessage();

        // Host Room
        PieceColor hostColor = PieceColor.White;
        await hostClient.HostRoom(hostColor);
        byte[] roomHostedMessage = await hostClient.ReadServerMessage();
        int roomId = RoomHostedMessage.Decode(roomHostedMessage);

        // Connect Joiner
        GameClient joiningClient = new();
        await joiningClient.ConnectToServer();
        await joiningClient.ReadServerMessage();

        // Attempt to Join Room
        await joiningClient.JoinRoom(roomId);
        byte[] joinerResponse = await joiningClient.ReadServerMessage();
        ServerMessage joinerResponseCode = (ServerMessage)MessageHelpers.ReadCode(joinerResponse);


        // Cancel Host
        await hostClient.CancelHost();

        Assert.NotEmpty(joinerResponse);
        Assert.Equal(ServerMessage.StartGame, joinerResponseCode);
    }

}
