using GameLogic.Enums;
using GameLogic.Pieces;

namespace GameLogic.Moves;

/// <summary>
/// A Castle move. Describes movement of the King and Rook
/// </summary>
public record CastleMove : Move
{
    public override MoveType MoveType => MoveType.Castle;

    public Square RookFrom { get; }
    public Square RookTo { get; }


    public CastleMove(Square from, Square to, Square rookFrom, Square rookTo) 
        :base(from, to)
    {
        RookFrom = rookFrom;
        RookTo = rookTo;
    }


    public override void Apply(Board board)
    {
        ApplyWithoutUpdatingHistory(board);
        board.History.AddEntry(this, null);
    }    

    public override bool LeavesPlayerInCheck(Board board)
    {
        return false;
    }

    public override bool MovesSquare(Square square)
    {
        return square == From || square == RookFrom;
    }


    protected override void ApplyWithoutUpdatingHistory(Board board)
    {
        if (board.At(From) == null || board.At(RookFrom) == null)
        {
            throw new InvalidOperationException("The From / RookFrom square is empty");
        }

        board.MovePiece(From, To);
        board.MovePiece(RookFrom, RookTo);
    }

    protected override void UndoWithoutUpdatingHistory(Board board, IPiece? capturedPiece)
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