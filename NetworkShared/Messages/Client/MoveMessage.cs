using GameLogic.Enums;
using GameLogic.Interfaces;
using GameLogic.Moves;
using NetworkShared.Enums;

namespace NetworkShared.Messages.Client;

public class MoveMessage
{
    /// Encoded Message Structure:
    ///     Byte 0: Message code
    ///     Byte 1: MoveType
    ///     Byte 2: Row From
    ///     Byte 3: Column From
    ///     Byte 4: Row To
    ///     Byte 5: Column To
    ///     
    /// 
    /// Subsequent Bytes depend on MoveType:
    ///
    ///     CastleMove:
    ///         Byte 6: RookFrom Row
    ///         Byte 7: RookFrom Column
    ///         Byte 8: RookTo Row
    ///         Byte 9: RookTo Column
    ///
    ///     EnPassantMove:
    ///         Byte 6: Captured Row 
    ///         Byte 7: Captured Column
    ///
    ///     PromotionMove:
    ///         Byte 6: PromotedTo PieceType
    ///
    ///     StandardMove:
    ///         No extra bytes



    /// <summary>
    /// The ClientCode for a MoveMessage.
    /// </summary>
    public static ClientMessage Code { get; } = ClientMessage.Move;




    #region Public Methods

    /// <summary>
    /// Decodes an IMove from an encoded MoveMessage byte array.
    /// </summary>
    /// <param name="message">A byte array MoveMessage.</param>
    /// <returns>An IMove.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static IMove Decode(byte[] message)
    {
        MoveType moveType = (MoveType)message[1];

        return moveType switch
        {
            MoveType.Castle => DecodeCastleMove(message),
            MoveType.EnPassant => DecodeEnPassantMove(message),
            MoveType.Promotion => DecodePromotionMove(message),
            MoveType.Standard => DecodeStandardMove(message),
            _ => throw new ArgumentException(nameof(moveType), $"Not expected MoveType: {moveType}")
        };

    }

    /// <summary>
    /// Encodes an IMove to an encoded MoveMessage byte array.
    /// </summary>
    /// <param name="move">The IMove to encode.</param>
    /// <returns>A byte array MoveMessage.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static byte[] Encode(IMove move)
    {
        return move.MoveType switch
        {
            MoveType.Castle => EncodeCastleMove((CastleMove)move),
            MoveType.EnPassant => EncodeEnPassantMove((EnPassantMove)move),
            MoveType.Promotion => EncodePromotionMove((PromotionMove)move),
            MoveType.Standard => EncodeStandardMove((StandardMove)move),
            _ => throw new ArgumentException(nameof(move.MoveType), $"Not expected MoveType: {move.MoveType}")
        };
    }

    #endregion



    #region Decode Methods

    private static CastleMove DecodeCastleMove(byte[] msg)
    {
        return new CastleMove(
            from: (msg[2], msg[3]),
            to: (msg[4], msg[5]),
            rookFrom: (msg[6], msg[7]),
            rookTo: (msg[8], msg[9]));
    }


    private static EnPassantMove DecodeEnPassantMove(byte[] msg)
    {
        return new EnPassantMove(
            from: (msg[2], msg[3]),
            to: (msg[4], msg[5]),
            captured: (msg[6], msg[7]));
    }


    private static PromotionMove DecodePromotionMove(byte[] msg)
    {
        return new PromotionMove(
            from: (msg[2], msg[3]),
            to: (msg[4], msg[5]),
            promotedTo: (PieceType)msg[6]);
    }


    private static StandardMove DecodeStandardMove(byte[] msg)
    {
        return new StandardMove(
            from: (msg[2], msg[3]),
            to: (msg[4], msg[5]));
    }

    #endregion



    #region Encode Methods

    /// <summary>
    /// Encodes the common data stored in all IMove types.
    /// </summary>
    /// <param name="move">The IMove to encode,</param>
    /// <returns></returns>
    private static byte[] BaseEncode(IMove move)
    {
        return [
            (byte)Code,
            (byte)move.MoveType,
            (byte)move.From.row,
            (byte)move.From.col,
            (byte)move.To.row,
            (byte)move.To.col
        ];
    }


    private static byte[] EncodeCastleMove(CastleMove castleMove)
    {
        return [
            .. BaseEncode(castleMove),
            (byte)castleMove.RookFrom.row,
            (byte)castleMove.RookFrom.col,
            (byte)castleMove.RookTo.row,
            (byte)castleMove.RookTo.col
        ];
    }


    private static byte[] EncodeEnPassantMove(EnPassantMove enPassantMove)
    {
        return [
            .. BaseEncode(enPassantMove),
            (byte)enPassantMove.Captured.row,
            (byte)enPassantMove.Captured.col,
        ];
    }


    private static byte[] EncodePromotionMove(PromotionMove promotionMove)
    {
        return [
            .. BaseEncode(promotionMove),
            (byte)promotionMove.PromotedTo
        ];
    }


    private static byte[] EncodeStandardMove(StandardMove standardMove)
    {
        return BaseEncode(standardMove);
    }

    #endregion
}

