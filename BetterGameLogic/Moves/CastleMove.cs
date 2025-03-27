using BetterGameLogic.Enums;
using BetterGameLogic.Pieces;

namespace BetterGameLogic.Moves;

/// <summary>
/// A Castle move. Describes movement of the King and Rook
/// </summary>
public class CastleMove : IMove
{
    public MoveType MoveType { get; } = MoveType.Castle;

    /// <summary>
    /// The square the king moves from.
    /// </summary>
    public Square From { get; }

    /// <summary>
    /// The square the king moves to.
    /// </summary>
    public Square To { get; }

    /// <summary>
    /// The square the rook moves from.
    /// </summary>
    public Square RookFrom { get; }

    /// <summary>
    /// The square the rook moves to.
    /// </summary>
    public Square RookTo { get; }


    public CastleMove(Square from, Square to, Square rookFrom, Square rookTo) 
    {
        From = from;
        To = to;
        RookFrom = rookFrom;
        RookTo = rookTo;
    }


    public void Apply(Board board)
    {
        if (board.At(From) == null || board.At(RookFrom) == null)
        {
            throw new InvalidOperationException("The From / RookFrom square is empty");
        }

        board.MovePiece(From, To);
        board.MovePiece(RookFrom, RookTo);
    }

    public void Undo(Board board, IPiece? capturedPiece)
    {
        if (capturedPiece != null)
        {
            throw new ArgumentException("A CastleMove should not capture any pieces");
        }

        if (board.At(To) == null || board.At(RookTo) == null)
        {
            throw new InvalidOperationException("The To / KingTo square is empty");
        }

        board.MovePiece(To, From);
        board.MovePiece(RookTo, RookFrom);
    }

    public bool LeavesPlayerInCheck(Board board)
    {
        return false;
    }

    public bool MovesSquare(Square square)
    {
        return square == From || square == RookFrom;
    }

    public bool IsEquivalentTo(IMove move)
    {
        if (move.MoveType != MoveType)
        {
            return false;
        }

        CastleMove castleMove = (CastleMove)move;

        return castleMove.From == From &&
               castleMove.To == To &&
               castleMove.RookFrom == RookFrom &&
               castleMove.RookTo == RookTo;
    }    
}