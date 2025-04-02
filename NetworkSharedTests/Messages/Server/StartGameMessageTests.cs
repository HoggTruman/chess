using GameLogic.Enums;
using NetworkShared.Messages.Server;

namespace NetworkSharedTests.Messages.Server;

public class StartGameMessageTests
{
    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void Encode_Decode_ReturnsOriginal(PieceColor clientColor)
    {
        // Arrange + Act
        var encoded = StartGameMessage.Encode(clientColor);
        var decoded = StartGameMessage.Decode(encoded);

        // Assert
        Assert.Equal(clientColor, decoded);
    }
}

