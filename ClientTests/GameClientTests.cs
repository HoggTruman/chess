using Client;
using FluentAssertions;
using GameLogic.Enums;
using GameLogic.Moves;
using NetworkShared.Messages.Server;

namespace ClientTests;

public class GameClientTests
{
    #region ConnectToServer Tests

    [Fact]
    public async Task ConnectToServer_WhenServerNotRunning_InvokesCommunicationError()
    {
        // Arrange
        bool communicationErrorInvoked = false;
        GameClient gameClient = new();
        gameClient.CommunicationError += () => communicationErrorInvoked = true;

        // Act
        await gameClient.ConnectToServer();

        // Assert
        communicationErrorInvoked.Should().BeTrue();
    }

    #endregion



    #region HandleServerMessage Tests

    [Fact]
    public void HandleServerMessage_WithRoomHostedMessage_InvokesRoomHosted()
    {
        // Arrange
        int receivedRoomId = 0; 
        GameClient gameClient = new();
        gameClient.RoomHosted += id => receivedRoomId = id;

        int messageRoomId = 278395108;
        byte[] message = RoomHostedMessage.Encode(messageRoomId);

        // Act
        gameClient.HandleServerMessage(message);

        // Assert
        receivedRoomId.Should().Be(messageRoomId);
    }


    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void HandleServerMessage_WithStartGameMessage_InvokesStartGame(PieceColor messageColor)
    {
        // Arrange
        PieceColor receivedcolor = PieceColor.None; 
        GameClient gameClient = new();
        gameClient.StartGame += color => receivedcolor = color;

        byte[] message = StartGameMessage.Encode(messageColor);

        // Act
        gameClient.HandleServerMessage(message);

        // Assert
        receivedcolor.Should().Be(messageColor);
    }


    [Fact]
    public void HandleServerMessage_WithRoomNotFoundMessage_InvokesRoomNotFound()
    {
        // Arrange
        bool roomNotFoundInvoked = false; 
        GameClient gameClient = new();
        gameClient.RoomNotFound += () => roomNotFoundInvoked = true;

        byte[] message = RoomNotFoundMessage.Encode();

        // Act
        gameClient.HandleServerMessage(message);

        // Assert
        roomNotFoundInvoked.Should().BeTrue();
    }


    [Fact]
    public void HandleServerMessage_WithRoomFullMessage_InvokesRoomFull()
    {
        // Arrange
        bool roomFullInvoked = false; 
        GameClient gameClient = new();
        gameClient.RoomFull += () => roomFullInvoked = true;

        byte[] message = RoomFullMessage.Encode();

        // Act
        gameClient.HandleServerMessage(message);

        // Assert
        roomFullInvoked.Should().BeTrue();
    }


    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void HandleServerMessage_WithRoomClosedMessage_InvokesRoomClosed(PieceColor messageColor)
    {
        // Arrange
        PieceColor receivedcolor = PieceColor.None; 
        GameClient gameClient = new();
        gameClient.RoomClosed += color => receivedcolor = color;

        byte[] message = RoomClosedMessage.Encode(messageColor);

        // Act
        gameClient.HandleServerMessage(message);

        // Assert
        receivedcolor.Should().Be(messageColor);
    }


    [Fact]
    public void HandleServerMessage_WithMoveMessage_InvokesMoveReceived()
    {
        // Arrange
        IMove? receivedMove = null; 
        GameClient gameClient = new();
        gameClient.MoveReceived += move => receivedMove = move;


        StandardMove serverMove = new(new(4, 4), new(5, 4));
        byte[] message = ServerMoveMessage.Encode(serverMove);

        // Act
        gameClient.HandleServerMessage(message);

        // Assert
        Assert.NotNull(receivedMove);
        ((Move)receivedMove == serverMove).Should().BeTrue();
    }

    #endregion
}