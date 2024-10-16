using NetworkShared.Enums;

namespace NetworkShared.Messages.Server;

public class RoomFullMessage
{
    /// Encoded Message Structure:
    /// Byte 0: Message Length
    /// Byte 1: Message Code
    

    /// <summary>
    /// The number of bytes in the message (including the Length and Code bytes).
    /// </summary>
    public const int Length = 2;

    /// <summary>
    /// The ServerMessage message type.
    /// </summary>
    public const ServerMessage Code = ServerMessage.RoomFull;


    public static ServerMessage Decode()
    {
        return Code;
    }


    public static byte[] Encode()
    {
        byte codeByte = (byte)Code;
        return [Length, codeByte];
    }
}

