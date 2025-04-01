using BetterGameLogic.Enums;
using NetworkShared.Enums;

namespace NetworkShared.Messages.Server;

public class StartGameMessage
{
    /// Encoded Message Structure:
    /// Byte 0: Message Length
    /// Byte 1: Message Code
    /// Byte 2: PieceColor of the Client player


    /// <summary>
    /// The number of bytes in the message (including the Length and Code bytes).
    /// </summary>
    public const int Length = 3;

    /// <summary>
    /// The ServerMessage message type.
    /// </summary>
    public const ServerMessage Code = ServerMessage.StartGame;


    /// <summary>
    /// Decodes the player's PieceColor from a StartGameMessage byte array.
    /// </summary>
    /// <param name="message">A StartGameMessage byte array.</param>
    /// <returns>The PieceColor of the player.</returns>
    public static PieceColor Decode(byte[] message)
    {
        PieceColor clientColor = (PieceColor)message[2];
        return clientColor;
    }


    /// <summary>
    /// Encodes the player's PieceColor to a StartGameMessage byte array.
    /// </summary>
    /// <param name="clientColor">The PieceColor of the player.</param>
    /// <returns>A StartGameMessage byte array.</returns>
    public static byte[] Encode(PieceColor clientColor)
    {
        byte codeByte = (byte)Code;
        byte clientColorByte = (byte)clientColor;

        byte[] message = [Length, codeByte, clientColorByte];

        return message;
    }
}

