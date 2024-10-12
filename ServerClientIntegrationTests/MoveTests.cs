using Client;
using GameLogic.Enums;
using GameLogic.Interfaces;
using GameLogic.Moves;
using NetworkShared;
using NetworkShared.Enums;
using NetworkShared.Messages.Server;
using NetworkShared.Messages.Shared;
using Server;

namespace ServerClientIntegrationTests;

[Collection("ServerClientIntegrationTests")]
public sealed class MoveTests : IAsyncLifetime
{
    private GameServer _gameServer;
    private GameClient _hostClient;
    private GameClient _joiningClient;

    private const PieceColor HostColor = PieceColor.White;
    private const PieceColor JoinerColor = PieceColor.Black;

    public MoveTests()
    {
        _gameServer = new();
        _hostClient = new();
        _joiningClient = new();

        _gameServer.Start();
    }


    public async Task InitializeAsync()
    {
        // Connect Host
        await _hostClient.ConnectToServer();

        // Host Room
        PieceColor hostColor = PieceColor.White;
        await _hostClient.HostRoom(hostColor);
        byte[] roomHostedMessage = await _hostClient.ReadServerMessage();
        int roomId = RoomHostedMessage.Decode(roomHostedMessage);

        // Connect Joiner
        await _joiningClient.ConnectToServer();

        // Join Room and receive StartGame
        await _joiningClient.JoinRoom(roomId);
        byte[] joinerResponse = await _joiningClient.ReadServerMessage();
        ServerMessage joinerStartCode = MessageHelpers.ReadServerCode(joinerResponse);
        Assert.Equal(ServerMessage.StartGame, joinerStartCode);

        // Host read StartGame message
        byte[] hostStartGameMessage = await _hostClient.ReadServerMessage();
        ServerMessage hostStartCode = MessageHelpers.ReadServerCode(hostStartGameMessage);
        Assert.Equal(ServerMessage.StartGame, hostStartCode);
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
}
