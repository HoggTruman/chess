using NetworkShared;
using NetworkShared.Enums;
using NetworkShared.Messages.Server;

namespace NetworkSharedTests.Messages.Server;

public class RoomFullMessageTests
{
    [Fact]
    public void Decode_ReturnsCode()
    {
        // Arrange + Act
        var decoded = RoomFullMessage.Decode();

        // Assert
        Assert.Equal(RoomFullMessage.Code, decoded);
    }

    [Fact]
    public void Encode_ContainsCode()
    {
        // Arrange + Act
        var encoded = RoomFullMessage.Encode();
        var containedCode = MessageHelpers.ReadServerCode(encoded);

        // Assert
        Assert.Equal(RoomFullMessage.Code, containedCode);
    }

}

