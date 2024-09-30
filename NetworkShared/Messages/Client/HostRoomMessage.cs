using GameLogic.Enums;
using NetworkShared.Enums;

namespace NetworkShared.Messages.Server;

public class HostRoomMessage
{
    // Encoded Message Structure:
    // Byte 0: Message code
    // Bytes 1-4: roomId


    public static ClientMessage Code { get; } = ClientMessage.HostRoom;

    
    public static PieceColor Decode(byte[] message)
    {
        PieceColor hostColor = (PieceColor)message[1];
        return hostColor;
    }


    public static byte[] Encode(PieceColor hostColor)
    {
        byte codeByte = (byte)ClientMessage.HostRoom;
        byte hostColorByte = (byte)hostColor;

        byte[] message = [codeByte, hostColorByte];

        return message;
    }
}

