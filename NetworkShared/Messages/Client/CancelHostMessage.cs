using NetworkShared.Enums;

namespace NetworkShared.Messages.Client;

public class CancelHostMessage
{
    /// Encoded Message Structure:
    /// Byte 0: Message Length
    /// Byte 1: Message Code


    /// <summary>
    /// The number of bytes in the message (including the Length and Code bytes).
    /// </summary>
    public const int Length = 2;

    /// <summary>
    /// The ClientMessage message type.
    /// </summary>
    public const ClientMessage Code = ClientMessage.CancelHost;


    public static byte[] Encode()
    {
        byte codeByte = (byte)Code;
        return [Length, codeByte];
    }
}

