using BetterGameLogic.Enums;
using BetterGameLogic.Pieces;

namespace BetterGameLogic.Moves;


/// <summary>
/// A standard move, i.e. Not a Castle, Pawn Promotion or En Passant
/// </summary>
public record StandardMove : Move
{
    public override MoveType MoveType => MoveType.Standard;

    public StandardMove(Square from, Square to) 
        :base(from, to)
    {

    }
    

    protected override void ApplyWithoutUpdatingHistory(Board board)
    {
        if (board.At(From) == null)
        {
            throw new InvalidOperationException("The From square is empty");
        }

        board.RemoveAt(To);
        board.MovePiece(From, To);
    }

    protected override void UndoWithoutUpdatingHistory(Board board, IPiece? capturedPiece)
    {
        if (board.At(To) == null)
        {
            throw new InvalidOperationException("The To square is empty");
        }

        board.MovePiece(To, From);

        if (capturedPiece != null)
        {
            board.AddPiece(capturedPiece);
        }        
    }
}