using BetterGameLogic.Enums;

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
        throw new NotImplementedException();
    }

    public void Undo(Board board)
    {
        throw new NotImplementedException();
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