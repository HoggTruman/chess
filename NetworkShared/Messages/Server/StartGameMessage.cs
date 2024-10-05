using GameLogic.Enums;
using NetworkShared.Enums;

namespace NetworkShared.Messages.Server;

public class StartGameMessage
{
    // Encoded Message Structure:
    // Byte 0: Message code
    // Byte 1: PieceColor of the Client player


    public static ServerMessage Code { get; } = ServerMessage.StartGame;


    public static PieceColor Decode(byte[] message)
    {
        PieceColor clientColor = (PieceColor)message[1];
        return clientColor;
    }


    public static byte[] Encode(PieceColor clientColor)
    {
        byte codeByte = (byte)Code;
        byte clientColorByte = (byte)clientColor;

        byte[] message = [codeByte, clientColorByte];

        return message;
    }
}

