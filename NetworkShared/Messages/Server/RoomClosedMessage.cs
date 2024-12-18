﻿using GameLogic.Enums;
using NetworkShared.Enums;

namespace NetworkShared.Messages.Server;

public class RoomClosedMessage
{
    /// Encoded Message Structure:
    /// Byte 0: Message Length
    /// Byte 1: Message Code
    /// Byte 2: PieceColor of winner
    

    /// <summary>
    /// The number of bytes in the message (including the Length and Code bytes).
    /// </summary>
    public const int Length = 3;

    /// <summary>
    /// The ServerMessage message type.
    /// </summary>
    public const ServerMessage Code = ServerMessage.RoomClosed;


    /// <summary>
    /// Decodes the PieceColor of the game's winner from an encoded RoomClosedMessage byte array.
    /// </summary>
    /// <param name="message">A byte array RoomClosedMessage.</param>
    /// <returns>A PieceColor.</returns>
    public static PieceColor Decode(byte[] message)
    {
        PieceColor winnerColor = (PieceColor)message[2];
        return winnerColor;
    }


    /// <summary>
    /// Encodes the PieceColor of the game's winner to an encoded RoomClosedMessage byte array.
    /// </summary>
    /// <param name="winnerColor">The PieceColor of the winner.</param>
    /// <returns>A byte array.</returns>
    public static byte[] Encode(PieceColor winnerColor)
    {
        byte codeByte = (byte)Code;
        byte winnerByte = (byte)winnerColor;
        return [Length, codeByte, winnerByte];
    }
}

