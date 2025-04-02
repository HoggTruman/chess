using FluentAssertions;
using GameLogic.Enums;
using GameLogic.Moves;
using Moq;
using Server;
using Server.Interfaces;

namespace ServerTests;

public class RoomTests
{
    #region Room Construction Tests

    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void Constructs_PropertiesReturnCorrectly(PieceColor hostColor)
    {
        // Arrange
        var mockClient = new Mock<IClient>();
        var hostClient = mockClient.Object;
        
        // Act
        Room room = new(hostClient, hostColor);

        // Assert
        Assert.Equal(hostClient, room.Host);
        Assert.Contains(hostClient, room.Players);
        Assert.Equal(hostColor, room.PlayerColors[hostClient]);
    }

    #endregion



    #region TryJoin Tests

    [Theory]
    [InlineData(PieceColor.White, PieceColor.Black)]
    [InlineData(PieceColor.Black, PieceColor.White)]
    public void TryJoin_WithOneClientInRoom_ReturnsTrue(PieceColor hostColor, PieceColor joinerColor)
    {
        // Arrange
        var mockHost = new Mock<IClient>();
        var hostClient = mockHost.Object;
        var mockJoiner = new Mock<IClient>();
        var joinerClient = mockJoiner.Object;

        Room room = new(hostClient, hostColor);

        // Act
        bool result = room.TryJoin(joinerClient);

        // Assert
        Assert.True(result);
        Assert.Contains(joinerClient, room.Players);
        Assert.Equal(joinerColor, room.PlayerColors[joinerClient]);
    }


    [Fact]
    public void TryJoin_WhenRoomContainsTwoClients_ReturnsFalse()
    {
        // Arrange
        var mockHost = new Mock<IClient>();
        var hostClient = mockHost.Object;
        var mockJoiner = new Mock<IClient>();
        var joinerClient = mockJoiner.Object;
        var mockThird = new Mock<IClient>();
        var thirdClient = mockThird.Object;

        Room room = new(hostClient, PieceColor.White);
        room.TryJoin(joinerClient);
        
        // Act
        bool result = room.TryJoin(thirdClient);

        // Assert
        Assert.False(result);
        room.Players.Should().HaveCount(2);
        room.PlayerColors.Keys.Should().HaveCount(2);
    }

    #endregion



    #region GetOpponent Tests

    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void GetOpponent_WithFullRoom_ReturnsOpponent(PieceColor hostColor)
    {
        // Arrange
        var mockHost = new Mock<IClient>();
        var hostClient = mockHost.Object;
        var mockJoiner = new Mock<IClient>();
        var joinerClient = mockJoiner.Object;

        Room room = new(hostClient, hostColor);
        room.TryJoin(joinerClient);

        // Act
        var hostOpponent = room.GetOpponent(hostClient);
        var joinerOpponent = room.GetOpponent(joinerClient);

        // Assert
        hostOpponent.Should().Be(joinerClient);
        joinerOpponent.Should().Be(hostClient);
    }


    [Fact]
    public void GetOpponent_WithOnePlayerInRoom_Throws()
    {
        // Arrange
        var mockHost = new Mock<IClient>();
        var hostClient = mockHost.Object;

        Room room = new(hostClient, PieceColor.White);

        // Act + Assert
        Assert.Throws<Exception>(() => room.GetOpponent(hostClient));
    }

    #endregion



    #region GetOpponentColor Tests

    [Theory]
    [InlineData(PieceColor.White, PieceColor.Black)]
    [InlineData(PieceColor.Black, PieceColor.White)]
    public void GetOpponentColor_ReturnsCorrectColor(PieceColor hostColor, PieceColor joinerColor)
    {
        // Arrange
        var mockHost = new Mock<IClient>();
        var hostClient = mockHost.Object;
        var mockJoiner = new Mock<IClient>();
        var joinerClient = mockJoiner.Object;

        Room room = new(hostClient, hostColor);
        room.TryJoin(joinerClient);

        // Act
        var hostOppColor = room.GetOpponentColor(hostClient);
        var joinerOppColor = room.GetOpponentColor(joinerClient);

        // Assert
        hostOppColor.Should().Be(joinerColor);
        joinerOppColor.Should().Be(hostColor);
    }

    #endregion



    #region TryMove Tests

    [Fact]
    public void TryMove_WithValidMove_ReturnsTrue()
    {
        // Arrange
        var mockHost = new Mock<IClient>();
        var hostClient = mockHost.Object;
        var mockJoiner = new Mock<IClient>();
        var joinerClient = mockJoiner.Object;

        Room room = new(hostClient, PieceColor.White);
        room.TryJoin(joinerClient);

        StandardMove move = new(new(6, 1), new(5, 1));

        // Act
        bool result = room.TryMove(hostClient, move);

        // Assert
        result.Should().BeTrue();
    }


    [Theory]
    [InlineData(6, 1, 3, 1)]
    [InlineData(4, 4, 4, 3)]
    [InlineData(1, 1, 2, 1)]
    public void TryMove_WithInvalidMove_ReturnsFalse(int fromRow, int fromCol, int toRow, int toCol)
    {
        // Arrange
        var mockHost = new Mock<IClient>();
        var hostClient = mockHost.Object;
        var mockJoiner = new Mock<IClient>();
        var joinerClient = mockJoiner.Object;

        Room room = new(hostClient, PieceColor.White);
        room.TryJoin(joinerClient);

        StandardMove move = new(new(fromRow, fromCol), new(toRow, toCol));

        // Act
        bool result = room.TryMove(hostClient, move);

        // Assert
        result.Should().BeFalse();
    }


    [Fact]
    public void TryMove_WhenNotPlayersTurn_ReturnsFalse()
    {
        // Arrange
        var mockHost = new Mock<IClient>();
        var hostClient = mockHost.Object;
        var mockJoiner = new Mock<IClient>();
        var joinerClient = mockJoiner.Object;

        Room room = new(hostClient, PieceColor.Black);
        room.TryJoin(joinerClient);

        StandardMove move = new(new(1, 1), new(2, 1));

        // Act
        bool result = room.TryMove(hostClient, move);

        // Assert
        result.Should().BeFalse();
    }

    #endregion
}
