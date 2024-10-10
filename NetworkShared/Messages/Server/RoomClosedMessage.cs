using GameLogic.Enums;
using NetworkShared.Enums;

namespace NetworkShared.Messages.Server;

public class RoomClosedMessage
{
    /// Encoded Message Structure:
    /// Byte 0: Message code
    /// Byte 1: PieceColor of winner
    

    public static ServerMessage Code { get; } = ServerMessage.RoomClosed;


    public static PieceColor Decode(byte[] message)
    {
        PieceColor winnerColor = (PieceColor)message[1];
        return winnerColor;
    }

    public static byte[] Encode(PieceColor winnerColor)
    {
        byte codeByte = (byte)Code;
        byte winnerByte = (byte)winnerColor;
        return [codeByte, winnerByte];
    }
}

