using GameLogic.Enums;
using NetworkShared.Messages.Client;

namespace NetworkSharedTests.Messages.Client;

public class HostRoomMessageTests
{
    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void Encode_Decode_ReturnsOriginal(PieceColor hostColor)
    {
        // Arrange + Act
        var encoded = HostRoomMessage.Encode(hostColor);
        var decoded = HostRoomMessage.Decode(encoded);

        // Assert
        Assert.Equal(hostColor, decoded);
    }
}

