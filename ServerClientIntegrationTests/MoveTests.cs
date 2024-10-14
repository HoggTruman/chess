using Client;
using GameLogic.Enums;
using GameLogic.Interfaces;
using GameLogic.Moves;
using NetworkShared;
using NetworkShared.Enums;
using NetworkShared.Messages.Server;
using NetworkShared.Messages.Shared;
using Server;
using System.Net.Sockets;
using System.Net;

namespace ServerClientIntegrationTests;

[Collection("ServerClientIntegrationTests")]
public sealed class MoveTests : IAsyncLifetime
{
    private readonly GameServer _gameServer;
    private readonly GameClient _hostClient;
    private readonly GameClient _joiningClient;

    private const PieceColor HostColor = PieceColor.White;
    private const PieceColor JoinerColor = PieceColor.Black;

    public MoveTests()
    {
        IPEndPoint ipEndPoint = new(IPAddress.Parse(ServerInfo.IpAddress), ServerInfo.Port);
        TcpListener tcpListener = new(ipEndPoint);
        _gameServer = new GameServer(tcpListener);
        _hostClient = new GameClient();
        _joiningClient = new GameClient();

        _gameServer.Start();
    }


    public async Task InitializeAsync()
    {
        // Connect Host
        await _hostClient.ConnectToServer();

        // Host Room
        await _hostClient.HostRoom(HostColor);
        byte[] roomHostedMessage = await _hostClient.ReadServerMessage();
        int roomId = RoomHostedMessage.Decode(roomHostedMessage);

        // Connect Joiner
        await _joiningClient.ConnectToServer();

        // Join Room and receive StartGame
        await _joiningClient.JoinRoom(roomId);
        byte[] joinerStartGameMessage = await _joiningClient.ReadServerMessage();
        ServerMessage joinerStartCode = MessageHelpers.ReadServerCode(joinerStartGameMessage);
        Assert.Equal(ServerMessage.StartGame, joinerStartCode);
        PieceColor receivedJoinerColor = StartGameMessage.Decode(joinerStartGameMessage);
        Assert.Equal(JoinerColor, receivedJoinerColor);        

        // Host read StartGame message
        byte[] hostStartGameMessage = await _hostClient.ReadServerMessage();
        ServerMessage hostStartCode = MessageHelpers.ReadServerCode(hostStartGameMessage);
        Assert.Equal(ServerMessage.StartGame, hostStartCode);
        PieceColor receivedHostColor = StartGameMessage.Decode(hostStartGameMessage);
        Assert.Equal(HostColor, receivedHostColor);
    }


    public async Task DisposeAsync()
    {
        await _gameServer.ShutDown();
    }




    [Fact(Timeout = 1000)]
    public async void ValidMoves_ReceivedSuccessfully()
    {
        // Host send move (host is white so moves first)
        StandardMove hostMove = new((6, 7), (4, 7));
        await _hostClient.Move(hostMove);

        // Joiner receives move from server
        byte[] joinerReceivedMove = await _joiningClient.ReadServerMessage();
        IMove joinerDecodedMove = MoveMessage.Decode(joinerReceivedMove);
        Assert.True(joinerDecodedMove.IsEquivalentTo(hostMove));

        // Joiner sends move
        StandardMove joinerMove = new((1, 0), (3, 0));
        await _joiningClient.Move(joinerMove);

        // Host receives move from server
        byte[] hostReceivedMove = await _hostClient.ReadServerMessage();
        IMove hostDecodedMove = MoveMessage.Decode(hostReceivedMove);
        Assert.True(hostDecodedMove.IsEquivalentTo(joinerMove));
    }


    [Fact(Timeout = 1000)]
    public async void SendValidMove_WhenNotPlayersTurn_ClosesRoom()
    {
        // Joiner sends move
        StandardMove joinerMove = new((1, 0), (3, 0));
        await _joiningClient.Move(joinerMove);

        // Host reads message from server
        byte[] hostReceivedMsg = await _hostClient.ReadServerMessage();
        ServerMessage hostReceivedCode = MessageHelpers.ReadServerCode(hostReceivedMsg);
        Assert.Equal(ServerMessage.RoomClosed, hostReceivedCode);

        PieceColor hostDecodedWinner = RoomClosedMessage.Decode(hostReceivedMsg);
        Assert.Equal(HostColor, hostDecodedWinner);

        // Joiner reads message from server
        byte[] joinerReceivedMsg = await _joiningClient.ReadServerMessage();
        ServerMessage joinerReceivedCode = MessageHelpers.ReadServerCode(joinerReceivedMsg);
        Assert.Equal(ServerMessage.RoomClosed, joinerReceivedCode);

        PieceColor joinerDecodedWinner = RoomClosedMessage.Decode(joinerReceivedMsg);
        Assert.Equal(HostColor, joinerDecodedWinner);
    }


    [Fact(Timeout = 1000)]
    public async void SendInvalidMove_WhenPlayersTurn_ClosesRoom()
    {
        // Host send move (host is white so moves first)
        StandardMove invalidMove = new((6, 7), (3, 7)); // pawn moving forward 3 squares
        await _hostClient.Move(invalidMove);

        // Host reads message from server
        byte[] hostReceivedMsg = await _hostClient.ReadServerMessage();
        ServerMessage hostReceivedCode = MessageHelpers.ReadServerCode(hostReceivedMsg);
        Assert.Equal(ServerMessage.RoomClosed, hostReceivedCode);

        PieceColor hostDecodedWinner = RoomClosedMessage.Decode(hostReceivedMsg);
        Assert.Equal(JoinerColor, hostDecodedWinner);

        // Joiner reads message from server
        byte[] joinerReceivedMsg = await _joiningClient.ReadServerMessage();
        ServerMessage joinerReceivedCode = MessageHelpers.ReadServerCode(joinerReceivedMsg);
        Assert.Equal(ServerMessage.RoomClosed, joinerReceivedCode);

        PieceColor joinerDecodedWinner = RoomClosedMessage.Decode(joinerReceivedMsg);
        Assert.Equal(JoinerColor, joinerDecodedWinner);
    }


    [Fact(Timeout = 1000)]
    public async void TryToMoveEnemyPiece_WhenPlayersTurn_ClosesRoom()
    {
        // Host send move (host is white so moves first)
        StandardMove enemyPieceMove = new((1, 0), (3, 0)); // move enemy pawn forward two
        await _hostClient.Move(enemyPieceMove);

        // Host reads message from server
        byte[] hostReceivedMsg = await _hostClient.ReadServerMessage();
        ServerMessage hostReceivedCode = MessageHelpers.ReadServerCode(hostReceivedMsg);
        Assert.Equal(ServerMessage.RoomClosed, hostReceivedCode);

        PieceColor hostDecodedWinner = RoomClosedMessage.Decode(hostReceivedMsg);
        Assert.Equal(JoinerColor, hostDecodedWinner);

        // Joiner reads message from server
        byte[] joinerReceivedMsg = await _joiningClient.ReadServerMessage();
        ServerMessage joinerReceivedCode = MessageHelpers.ReadServerCode(joinerReceivedMsg);
        Assert.Equal(ServerMessage.RoomClosed, joinerReceivedCode);

        PieceColor joinerDecodedWinner = RoomClosedMessage.Decode(joinerReceivedMsg);
        Assert.Equal(JoinerColor, joinerDecodedWinner);
    }


    [Fact(Timeout = 1000)]
    public async void RoomClosed_WhenGameOver()
    {
        ///
        ///

        // Move pawn in front of king
        StandardMove whiteMove1 = new((6, 4), (4, 4));
        await _hostClient.Move(whiteMove1);

        // Joiner receives and moves edge pawn
        byte[] receivedWhiteMove1 = await _joiningClient.ReadServerMessage();
        StandardMove blackMove1 = new((1, 0), (2, 0));
        await _joiningClient.Move(blackMove1);

        // Host receives and moves bishop
        byte[] receivedBlackMove1 = await _hostClient.ReadServerMessage();
        StandardMove whiteMove2 = new((7, 5), (4, 2));
        await _hostClient.Move(whiteMove2);

        // Joiner receives and moves edge pawn
        byte[] receivedWhiteMove2 = await _joiningClient.ReadServerMessage();
        StandardMove blackMove2 = new((2, 0), (3, 0));
        await _joiningClient.Move(blackMove2);

        // Host receives and moves queen
        byte[] receivedBlackMove2 = await _hostClient.ReadServerMessage();
        StandardMove whiteMove3 = new((7, 3), (5, 5));
        await _hostClient.Move(whiteMove3);

        // Joiner receives and moves edge pawn
        byte[] receivedWhiteMove3 = await _joiningClient.ReadServerMessage();
        StandardMove blackMove3 = new((3, 0), (4, 0));
        await _joiningClient.Move(blackMove3);

        // Host receives and checkmates with queen
        byte[] receivedBlackMove3 = await _hostClient.ReadServerMessage();
        StandardMove whiteMove4 = new((5, 5), (1, 5));
        await _hostClient.Move(whiteMove4);


        await Task.Delay(100); // waits for server to finish processing. The Move and RoomClosed messages should both be available at the same time

        // Joiner receives
        byte[] receivedWhiteMove4 = await _joiningClient.ReadServerMessage();

        // Receive RoomClosedMessage on host
        byte[] hostRoomClosedMessage = await _hostClient.ReadServerMessage();
        ServerMessage hostMessageCode = MessageHelpers.ReadServerCode(hostRoomClosedMessage);
        Assert.Equal(ServerMessage.RoomClosed, hostMessageCode);

        PieceColor hostWinnerColor = RoomClosedMessage.Decode(hostRoomClosedMessage);
        Assert.Equal(HostColor, hostWinnerColor);

        // Receive RoomClosedMessage on host
        byte[] joinerRoomClosedMessage = await _joiningClient.ReadServerMessage();
        ServerMessage joinerMessageCode = MessageHelpers.ReadServerCode(hostRoomClosedMessage);
        Assert.Equal(ServerMessage.RoomClosed, joinerMessageCode);

        PieceColor joinerWinnerColor = RoomClosedMessage.Decode(joinerRoomClosedMessage);
        Assert.Equal(HostColor, joinerWinnerColor);
    }
}
