using GameLogic.Moves;
using NetworkShared.Enums;
using NetworkShared.Messages.Shared;

namespace NetworkShared.Messages.Client;

public class ClientMoveMessage : MoveMessage
{
    /// Encoded Message Structure:
    ///     Byte 0: Message Length
    ///     Byte 1: Message Code
    ///     Byte 2: MoveType
    ///     Byte 3: Row From
    ///     Byte 4: Column From
    ///     Byte 5: Row To
    ///     Byte 6: Column To
    ///     
    /// 
    /// Subsequent Bytes depend on MoveType:
    ///
    ///     CastleMove:
    ///         Byte 7: RookFrom Row
    ///         Byte 8: RookFrom Column
    ///         Byte 9: RookTo Row
    ///         Byte 10: RookTo Column
    ///
    ///     EnPassantMove:
    ///         Byte 7: Captured Row 
    ///         Byte 8: Captured Column
    ///
    ///     PromotionMove:
    ///         Byte 7: PromotedTo PieceType
    ///
    ///     StandardMove:
    ///         No extra bytes


    /// <summary>
    /// The ClientMessage message type.
    /// </summary>
    public const ClientMessage Code = ClientMessage.Move;



    #region Public Methods

    /// <summary>
    /// Decodes an IMove from an encoded ClientMoveMessage byte array.
    /// </summary>
    /// <param name="message">A byte array ClientMoveMessage.</param>
    /// <returns>An IMove.</returns>
    /// <exception cref="ArgumentException"></exception>
    new public static IMove Decode(byte[] message)
    {
        return MoveMessage.Decode(message);
    }


    /// <summary>
    /// Encodes an IMove to an encoded ClientMoveMessage byte array.
    /// </summary>
    /// <param name="move">The IMove to encode.</param>
    /// <returns>A byte array ClientMoveMessage.</returns>
    /// <exception cref="ArgumentException"></exception>
    new public static byte[] Encode(IMove move)
    {
        byte[] message = MoveMessage.Encode(move);
        message[1] = (byte)Code;
        return message;
    }

    #endregion
}

