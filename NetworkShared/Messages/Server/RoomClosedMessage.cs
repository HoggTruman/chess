using NetworkShared.Enums;

namespace NetworkShared.Messages.Server;

public class RoomClosedMessage
{
    // Encoded Message Structure:
    // Byte 0: Message code
    

    public static ServerMessage Code { get; } = ServerMessage.RoomClosed;


    public static byte[] Encode()
    {
        byte codeByte = (byte)Code;
        return [codeByte];
    }
}

