using BetterGameLogic.Enums;
using BetterGameLogic.Pieces;

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

    public abstract void Undo(Board board, IPiece? capturedPiece);

    public virtual bool LeavesPlayerInCheck(Board board)
    {
        IPiece? movingPiece = board.At(From);

        if (movingPiece == null)
        {
            throw new InvalidOperationException("There is no piece on the From square.");
        }
        
        IPiece? captured = board.At(To);
        Apply(board);
        bool result = board.GetKing(movingPiece.Color).IsUnderCheck();
        Undo(board, captured);
        return result;
    }


    public bool MovesSquare(Square square)
    {
        return square == From;
    }

    public abstract bool IsEquivalentTo(IMove move);    
}