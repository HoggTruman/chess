using BetterGameLogic.Enums;
using BetterGameLogic.Moves;

namespace NetworkShared.Messages.Shared;

public abstract class MoveMessage
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
    /// The number of bytes in the message for a CastleMove.
    /// </summary>
    public const int CastleLength = 11;

    /// <summary>
    /// The number of bytes in the message for an EnPassantMove.
    /// </summary>
    public const int EnPassantLength = 9;

    /// <summary>
    /// The number of bytes in the message for a PromotionMove.
    /// </summary>
    public const int PromotionLength = 8;

    /// <summary>
    /// The number of bytes in the message for a StandardMove.
    /// </summary>
    public const int StandardLength = 7;



    #region Protected Methods

    /// <summary>
    /// Decodes an IMove from an encoded MoveMessage byte array.
    /// </summary>
    /// <param name="message">A byte array MoveMessage.</param>
    /// <returns>An IMove.</returns>
    /// <exception cref="ArgumentException"></exception>
    protected static IMove Decode(byte[] message)
    {
        MoveType moveType = (MoveType)message[2];

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
    protected static byte[] Encode(IMove move)
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
            from: new(msg[3], msg[4]),
            to: new(msg[5], msg[6]),
            rookFrom: new(msg[7], msg[8]),
            rookTo: new(msg[9], msg[10]));
    }


    private static EnPassantMove DecodeEnPassantMove(byte[] msg)
    {
        return new EnPassantMove(
            from: new(msg[3], msg[4]),
            to: new(msg[5], msg[6]));
    }


    private static PromotionMove DecodePromotionMove(byte[] msg)
    {
        return new PromotionMove(
            from: new(msg[3], msg[4]),
            to: new(msg[5], msg[6]),
            promotedTo: (PieceType)msg[7]);
    }


    private static StandardMove DecodeStandardMove(byte[] msg)
    {
        return new StandardMove(
            from: new(msg[3], msg[4]),
            to: new(msg[5], msg[6]));
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
            0, // Code goes here
            (byte)move.MoveType,
            (byte)move.From.Row,
            (byte)move.From.Col,
            (byte)move.To.Row,
            (byte)move.To.Col
        ];
    }


    private static byte[] EncodeCastleMove(CastleMove castleMove)
    {
        return [
            CastleLength,
            .. BaseEncode(castleMove),
            (byte)castleMove.RookFrom.Row,
            (byte)castleMove.RookFrom.Col,
            (byte)castleMove.RookTo.Row,
            (byte)castleMove.RookTo.Col
        ];
    }


    private static byte[] EncodeEnPassantMove(EnPassantMove enPassantMove)
    {
        return [
            EnPassantLength,
            .. BaseEncode(enPassantMove),
            (byte)enPassantMove.Captured.Row,
            (byte)enPassantMove.Captured.Col,
        ];
    }


    private static byte[] EncodePromotionMove(PromotionMove promotionMove)
    {
        return [
            PromotionLength,
            .. BaseEncode(promotionMove),
            (byte)promotionMove.PromotedTo
        ];
    }


    private static byte[] EncodeStandardMove(StandardMove standardMove)
    {
        return [
            StandardLength,
            .. BaseEncode(standardMove)
        ];
    }

    #endregion
}

