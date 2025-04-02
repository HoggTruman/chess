using BetterGameLogic.Enums;
using BetterGameLogic.Moves;
using Client;
using NetworkShared;
using NetworkShared.Enums;
using NetworkShared.Messages.Server;
using Server;
using System.Net;
using System.Net.Sockets;

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
        await _hostClient.SendHostRoom(HostColor);
        byte[] roomHostedMessage = await _hostClient.ReadServerMessage();
        int roomId = RoomHostedMessage.Decode(roomHostedMessage);

        // Connect Joiner
        await _joiningClient.ConnectToServer();

        // Join Room and receive StartGame
        await _joiningClient.SendJoinRoom(roomId);
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
    public async Task ValidMoves_ReceivedSuccessfully()
    {
        // Host send move (host is white so moves first)
        StandardMove hostMove = new(new(6, 7), new(4, 7));
        await _hostClient.SendMove(hostMove);

        // Joiner receives move from server
        byte[] joinerReceivedMove = await _joiningClient.ReadServerMessage();
        IMove joinerDecodedMove = ServerMoveMessage.Decode(joinerReceivedMove);
        Assert.True(hostMove.Equals(joinerDecodedMove));

        // Joiner sends move
        StandardMove joinerMove = new(new(1, 0), new(3, 0));
        await _joiningClient.SendMove(joinerMove);

        // Host receives move from server
        byte[] hostReceivedMove = await _hostClient.ReadServerMessage();
        IMove hostDecodedMove = ServerMoveMessage.Decode(hostReceivedMove);
        Assert.True(joinerMove.Equals(hostDecodedMove));
    }


    [Fact(Timeout = 1000)]
    public async Task SendValidMove_WhenNotPlayersTurn_ClosesRoom()
    {
        // Joiner sends move
        StandardMove joinerMove = new(new(1, 0), new(3, 0));
        await _joiningClient.SendMove(joinerMove);

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
    public async Task SendInvalidMove_WhenPlayersTurn_ClosesRoom()
    {
        // Host send move (host is white so moves first)
        StandardMove invalidMove = new(new(6, 7), new(3, 7)); // pawn moving forward 3 squares
        await _hostClient.SendMove(invalidMove);

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
    public async Task TryToMoveEnemyPiece_WhenPlayersTurn_ClosesRoom()
    {
        // Host send move (host is white so moves first)
        StandardMove enemyPieceMove = new(new(1, 0), new(3, 0)); // move enemy pawn forward two
        await _hostClient.SendMove(enemyPieceMove);

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
    public async Task RoomClosed_WhenGameOver()
    {
        /// Completes a very short game to checkmate, to test that the room is closed
        /// at the end of it.

        // Move pawn in front of king
        StandardMove whiteMove1 = new(new(6, 4), new(4, 4));
        await _hostClient.SendMove(whiteMove1);

        // Joiner receives and moves edge pawn
        byte[] receivedWhiteMove1 = await _joiningClient.ReadServerMessage();
        StandardMove blackMove1 = new(new(1, 0), new(2, 0));
        await _joiningClient.SendMove(blackMove1);

        // Host receives and moves bishop
        byte[] receivedBlackMove1 = await _hostClient.ReadServerMessage();
        StandardMove whiteMove2 = new(new(7, 5), new(4, 2));
        await _hostClient.SendMove(whiteMove2);

        // Joiner receives and moves edge pawn
        byte[] receivedWhiteMove2 = await _joiningClient.ReadServerMessage();
        StandardMove blackMove2 = new(new(2, 0), new(3, 0));
        await _joiningClient.SendMove(blackMove2);

        // Host receives and moves queen
        byte[] receivedBlackMove2 = await _hostClient.ReadServerMessage();
        StandardMove whiteMove3 = new(new(7, 3), new(5, 5));
        await _hostClient.SendMove(whiteMove3);

        // Joiner receives and moves edge pawn
        byte[] receivedWhiteMove3 = await _joiningClient.ReadServerMessage();
        StandardMove blackMove3 = new(new(3, 0), new(4, 0));
        await _joiningClient.SendMove(blackMove3);

        // Host receives and checkmates with queen
        byte[] receivedBlackMove3 = await _hostClient.ReadServerMessage();
        StandardMove whiteMove4 = new(new(5, 5), new(1, 5));
        await _hostClient.SendMove(whiteMove4);

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
