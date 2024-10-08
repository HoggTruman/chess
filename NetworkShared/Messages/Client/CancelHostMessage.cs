using NetworkShared.Enums;

namespace NetworkShared.Messages.Client;

public class CancelHostMessage
{
    // Encoded Message Structure:
    // Byte 0: Message code


    public static ClientMessage Code { get; } = ClientMessage.CancelHost;


    public static byte[] Encode()
    {
        byte codeByte = (byte)Code;
        return [codeByte];
    }
}

