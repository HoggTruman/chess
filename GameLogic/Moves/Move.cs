using GameLogic.Enums;
using GameLogic.Pieces;

namespace GameLogic.Moves;


public abstract record Move : IMove
{
    public abstract MoveType MoveType { get; }
    public Square From { get; }
    public Square To { get; }
    public virtual Square Captured => To;

    public Move(Square from, Square to)
    {
        From = from;
        To = to;
    }

    

    public virtual void Apply(Board board)
    {
        IPiece? capturedPiece = board.At(Captured);
        ApplyWithoutUpdatingHistory(board);
        board.History.AddEntry(this, capturedPiece);
    }

    public virtual bool LeavesPlayerInCheck(Board board)
    {
        IPiece? movingPiece = board.At(From);

        if (movingPiece == null)
        {
            throw new InvalidOperationException("There is no piece on the From square.");
        }
        
        IPiece? capturedPiece = board.At(Captured);
        ApplyWithoutUpdatingHistory(board);
        bool result = board.GetKing(movingPiece.Color).IsUnderCheck();
        UndoWithoutUpdatingHistory(board, capturedPiece);
        return result;
    }

    public virtual bool MovesSquare(Square square)
    {
        return square == From;
    }


    protected abstract void ApplyWithoutUpdatingHistory(Board board);

    protected abstract void UndoWithoutUpdatingHistory(Board board, IPiece? capturedPiece);
}