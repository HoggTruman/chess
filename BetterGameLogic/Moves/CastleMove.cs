using BetterGameLogic.Enums;
using BetterGameLogic.Pieces;

namespace BetterGameLogic.Moves;

/// <summary>
/// A Castle move. Describes movement of the King and Rook
/// </summary>
public record CastleMove : IMove
{
    public MoveType MoveType { get; } = MoveType.Castle;

    public Square From { get; }
    public Square To { get; }
    public Square RookFrom { get; }
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
        ApplyWithoutUpdatingHistory(board);
        board.History.AddEntry(this, null);
    }    

    public bool LeavesPlayerInCheck(Board board)
    {
        return false;
    }

    public bool MovesSquare(Square square)
    {
        return square == From || square == RookFrom;
    }


    protected void ApplyWithoutUpdatingHistory(Board board)
    {
        if (board.At(From) == null || board.At(RookFrom) == null)
        {
            throw new InvalidOperationException("The From / RookFrom square is empty");
        }

        board.MovePiece(From, To);
        board.MovePiece(RookFrom, RookTo);
    }

    protected void UndoWithoutUpdatingHistory(Board board, IPiece? capturedPiece)
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
}