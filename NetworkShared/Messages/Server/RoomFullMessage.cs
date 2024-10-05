using NetworkShared.Enums;

namespace NetworkShared.Messages.Server;

public class RoomFullMessage
{
    // Encoded Message Structure:
    // Byte 0: Message code
    

    public static ServerMessage Code { get; } = ServerMessage.RoomFull;


    public static ServerMessage Decode()
    {
        return Code;
    }


    public static byte[] Encode()
    {
        byte codeByte = (byte)Code;
        return [codeByte];
    }
}

