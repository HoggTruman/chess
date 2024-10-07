using NetworkShared.Messages.Client;

namespace NetworkSharedTests.Messages.Client;

public class JoinRoomMessageTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(532322)]
    public void Encode_Decode_ReturnsOriginal(int roomId)
    {
        // Arrange + Act
        var encoded = JoinRoomMessage.Encode(roomId);
        var decoded = JoinRoomMessage.Decode(encoded);

        // Assert
        Assert.Equal(roomId, decoded);
    }
}

