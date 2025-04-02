using GameLogic.Enums;
using GameLogic.Pieces;

namespace GameLogic.Moves;

/// <summary>
/// An En Passant move. Contains the captured Pawn's square
/// </summary>
public record EnPassantMove : Move
{
    public override MoveType MoveType => MoveType.EnPassant;
    public override Square Captured => new(From.Row, To.Col);

    public EnPassantMove(Square from, Square to) 
        :base(from, to)
    {
        
    }


    protected override void ApplyWithoutUpdatingHistory(Board board)
    {
        if (board.At(From) == null)
        {
            throw new InvalidOperationException("The From square is empty");
        }

        board.RemoveAt(Captured);
        board.MovePiece(From, To);
    }

    protected override void UndoWithoutUpdatingHistory(Board board, IPiece? capturedPiece)
    {
        if (capturedPiece == null)
        {
            throw new ArgumentException("capturedPiece can not be null for an EnPassantMove");
        }

        if (board.At(To) == null)
        {
            throw new InvalidOperationException("The To square is empty");
        }

        board.MovePiece(To, From);
        board.AddPiece(capturedPiece);        
    }
}