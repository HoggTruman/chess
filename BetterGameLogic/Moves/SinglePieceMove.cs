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

    public virtual bool LeavesPlayerInCheck(Board board)
    {
        IPiece? movingPiece = board.At(From);

        if (movingPiece == null)
        {
            throw new InvalidOperationException("There is no piece on the From square.");
        }
        
        IPiece? captured = board.At(To);
        ApplyWithoutUpdatingHistory(board);
        bool result = board.GetKing(movingPiece.Color).IsUnderCheck();
        UndoWithoutUpdatingHistory(board, captured);
        return result;
    }


    public bool MovesSquare(Square square)
    {
        return square == From;
    }

    public abstract bool IsEquivalentTo(IMove move);

    protected abstract void ApplyWithoutUpdatingHistory(Board board);

    protected abstract void UndoWithoutUpdatingHistory(Board board, IPiece? capturedPiece);
}