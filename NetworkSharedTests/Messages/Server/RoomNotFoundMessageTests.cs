using NetworkShared;
using NetworkShared.Enums;
using NetworkShared.Messages.Server;

namespace NetworkSharedTests.Messages.Server;

public class RoomNotFoundMessageTests
{
    [Fact]
    public void Decode_ReturnsCode()
    {
        // Arrange + Act
        var decoded = RoomNotFoundMessage.Decode();

        // Assert
        Assert.Equal(RoomNotFoundMessage.Code, decoded);
    }

    [Fact]
    public void Encode_ContainsCode()
    {
        // Arrange + Act
        var encoded = RoomNotFoundMessage.Encode();
        var containedCode = MessageHelpers.ReadServerCode(encoded);

        // Assert
        Assert.Equal(RoomNotFoundMessage.Code, containedCode);
    }

}

