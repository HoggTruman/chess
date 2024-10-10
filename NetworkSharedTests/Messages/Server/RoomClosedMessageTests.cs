using GameLogic.Enums;
using NetworkShared.Messages.Server;

namespace NetworkSharedTests.Messages.Server;

public class RoomClosedMessageTests
{
    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    [InlineData(PieceColor.None)]
    public void Encode_Decode_ReturnsOriginal(PieceColor winnerColor)
    {
        // Arrange + Act
        var encoded = StartGameMessage.Encode(winnerColor);
        var decoded = StartGameMessage.Decode(encoded);

        // Assert
        Assert.Equal(winnerColor, decoded);
    }
}

