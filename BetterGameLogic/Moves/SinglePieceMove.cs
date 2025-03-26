using BetterGameLogic.Enums;

namespace BetterGameLogic.Moves;

/// <summary>
/// An abstract class for moves that only move a single piece (any move other than castling)
/// </summary>
public abstract class SinglePieceMove : IMove
{
    public abstract MoveType MoveType { get; }
    public Square From { get; }
    public Square To { get; }

    public SinglePieceMove(Square from, Square to)
    {
        From = from;
        To = to;
    }


    public abstract void Apply(Board board);    

    public abstract void Undo(Board board);


    public bool MovesSquare(Square square)
    {
        return square == From;
    }

    public abstract bool IsEquivalentTo(IMove move);    
}