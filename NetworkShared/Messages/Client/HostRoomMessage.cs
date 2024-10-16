using GameLogic.Enums;
using NetworkShared.Enums;

namespace NetworkShared.Messages.Client;

public class HostRoomMessage
{
    /// Encoded Message Structure:
    /// Byte 0: Message Length
    /// Byte 1: Message Code
    /// Byte 2: PieceColor of the host


    /// <summary>
    /// The number of bytes in the message (including the Length and Code bytes).
    /// </summary>
    public const int Length = 3;

    /// <summary>
    /// The ClientMessage message type.
    /// </summary>
    public const ClientMessage Code = ClientMessage.HostRoom;

    
    public static PieceColor Decode(byte[] message)
    {
        PieceColor hostColor = (PieceColor)message[2];
        return hostColor;
    }


    public static byte[] Encode(PieceColor hostColor)
    {
        byte codeByte = (byte)ClientMessage.HostRoom;
        byte hostColorByte = (byte)hostColor;

        byte[] message = [
            Length,
            codeByte, 
            hostColorByte
        ];

        return message;
    }
}

