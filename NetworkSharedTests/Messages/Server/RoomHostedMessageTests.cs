using GameLogic.Enums;
using NetworkShared.Messages.Server;

namespace NetworkSharedTests.Messages.Server;

public class RoomHostedMessageTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(532)]
    public void Encode_Decode_ReturnsOriginal(int roomId)
    {
        // Arrange + Act
        var encoded = RoomHostedMessage.Encode(roomId);
        var decoded = RoomHostedMessage.Decode(encoded);

        // Assert
        Assert.Equal(roomId, decoded);
    }
}

