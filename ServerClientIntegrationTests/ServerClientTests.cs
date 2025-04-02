using Client;
using BetterGameLogic.Enums;
using BetterGameLogic.Helpers;
using NetworkShared;
using NetworkShared.Enums;
using NetworkShared.Messages.Server;
using Server;
using System.Net.Sockets;
using System.Net;

namespace ServerClientIntegrationTests;

[Collection("ServerClientIntegrationTests")]
public sealed class ServerClientTests : IAsyncLifetime
{
    private readonly GameServer _gameServer;

    public ServerClientTests()
    {
        IPEndPoint ipEndPoint = new(IPAddress.Parse(ServerInfo.IpAddress), ServerInfo.Port);
        TcpListener tcpListener = new(ipEndPoint);
        _gameServer = new GameServer(tcpListener);
        _gameServer.Start();
    }


    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }


    public async Task DisposeAsync()
    {
        await _gameServer.ShutDown();
    }




    [Fact(Timeout = 1000)]
    public async void ServerStartsAndShutsDownWithClient()
    {
        GameClient gameClient = new();
        await gameClient.ConnectToServer();
        await _gameServer.ShutDown();
    }


    [Fact(Timeout = 1000)]
    public async void ClientSendsHostRoom_ReceivesRoomHosted()
    {
        GameClient gameClient = new();
        await gameClient.ConnectToServer();

        await gameClient.SendHostRoom(PieceColor.White);
        byte[] roomHostedMessage = await gameClient.ReadServerMessage();
        int roomId = RoomHostedMessage.Decode(roomHostedMessage);

        Assert.True(roomId > 0);

    }


    [Fact(Timeout = 1000)]
    public async void ClientSendsJoinRoom_WithNoHostedRooms_ReceivesRoomNotFound()
    {
        GameClient gameClient = new();
        await gameClient.ConnectToServer();

        int roomId = 1234;
        await gameClient.SendJoinRoom(roomId);
        byte[] roomNotFoundMessage = await gameClient.ReadServerMessage();
        ServerMessage responseCode = MessageHelpers.ReadServerCode(roomNotFoundMessage);

        Assert.NotEmpty(roomNotFoundMessage);
        Assert.Equal(ServerMessage.RoomNotFound, responseCode);
    }


    [Fact(Timeout = 1000)]
    public async void ClientSendsJoinRoom_WithRoomAlreadyFull_ReceivesRoomFull()
    {
        // Connect Clients
        GameClient hostClient = new();
        GameClient joiningClient = new();
        GameClient testClient = new();

        await hostClient.ConnectToServer();
        await joiningClient.ConnectToServer();
        await testClient.ConnectToServer();

        // Host Room
        await hostClient.SendHostRoom(PieceColor.White);
        byte[] roomHostedMessage = await hostClient.ReadServerMessage();
        int roomId = RoomHostedMessage.Decode(roomHostedMessage);

        // Join Room
        await joiningClient.SendJoinRoom(roomId);
        byte[] joinerResponseMessage = await joiningClient.ReadServerMessage();

        // Attempt to join the full room
        await testClient.SendJoinRoom(roomId);
        byte[] testResponseMessage = await testClient.ReadServerMessage();
        ServerMessage responseCode = MessageHelpers.ReadServerCode(testResponseMessage);

        Assert.NotEmpty(testResponseMessage);
        Assert.Equal(ServerMessage.RoomFull, responseCode);
    }


    [Fact(Timeout = 1000)]
    public async void ClientJoinsRoom_HostAndJoinerReceiveStartGame_WithCorrectColors()
    {
        // Connect Clients
        GameClient hostClient = new();
        GameClient joiningClient = new();

        await hostClient.ConnectToServer();
        await joiningClient.ConnectToServer();

        // Host Room
        PieceColor hostColor = PieceColor.White;
        await hostClient.SendHostRoom(hostColor);
        byte[] roomHostedMessage = await hostClient.ReadServerMessage();
        int roomId = RoomHostedMessage.Decode(roomHostedMessage);

        // Join Room
        await joiningClient.SendJoinRoom(roomId);
        byte[] joinerResponse = await joiningClient.ReadServerMessage();
        ServerMessage joinerResponseCode = MessageHelpers.ReadServerCode(joinerResponse);

        Assert.NotEmpty(joinerResponse);
        Assert.Equal(ServerMessage.StartGame, joinerResponseCode);

        PieceColor joinerResponseColor = StartGameMessage.Decode(joinerResponse);
        Assert.Equal(ColorHelpers.Opposite(hostColor), joinerResponseColor);

        // Host Response
        byte[] hostResponse = await hostClient.ReadServerMessage();        
        ServerMessage hostResponseCode = MessageHelpers.ReadServerCode(hostResponse);       
        
        Assert.NotEmpty(hostResponse);
        Assert.Equal(ServerMessage.StartGame, hostResponseCode);

        PieceColor hostResponseColor = StartGameMessage.Decode(hostResponse);
        Assert.Equal(hostColor, hostResponseColor);
    }


    [Fact(Timeout = 1000)]
    public async void RoomCancelled_BeforeJoined_JoinerGetsRoomNotFound()
    {
        // Connect Host
        GameClient hostClient = new();
        await hostClient.ConnectToServer();

        // Host Room
        PieceColor hostColor = PieceColor.White;
        await hostClient.SendHostRoom(hostColor);
        byte[] roomHostedMessage = await hostClient.ReadServerMessage();
        int roomId = RoomHostedMessage.Decode(roomHostedMessage);

        // Cancel Host
        await hostClient.SendCancelHost();
        await Task.Delay(10); // Make sure the room is fully closed down

        // Connect Joiner
        GameClient joiningClient = new();
        await joiningClient.ConnectToServer();

        // Attempt to Join Room
        await joiningClient.SendJoinRoom(roomId);
        byte[] joinerResponse = await joiningClient.ReadServerMessage();
        ServerMessage joinerResponseCode = MessageHelpers.ReadServerCode(joinerResponse);

        Assert.NotEmpty(joinerResponse);
        Assert.Equal(ServerMessage.RoomNotFound, joinerResponseCode);
    }


    [Fact(Timeout = 1000)]
    public async void RoomCancelled_AfterJoined_JoinerGetsStartGame()
    {
        // Connect Host
        GameClient hostClient = new();
        await hostClient.ConnectToServer();

        // Host Room
        PieceColor hostColor = PieceColor.White;
        await hostClient.SendHostRoom(hostColor);
        byte[] roomHostedMessage = await hostClient.ReadServerMessage();
        int roomId = RoomHostedMessage.Decode(roomHostedMessage);

        // Connect Joiner
        GameClient joiningClient = new();
        await joiningClient.ConnectToServer();

        // Attempt to Join Room
        await joiningClient.SendJoinRoom(roomId);
        byte[] joinerResponse = await joiningClient.ReadServerMessage();
        ServerMessage joinerResponseCode = MessageHelpers.ReadServerCode(joinerResponse);


        // Cancel Host
        await hostClient.SendCancelHost();

        Assert.NotEmpty(joinerResponse);
        Assert.Equal(ServerMessage.StartGame, joinerResponseCode);
    }
}
