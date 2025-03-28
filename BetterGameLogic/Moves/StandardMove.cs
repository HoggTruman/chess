using BetterGameLogic.Enums;
using BetterGameLogic.Pieces;

namespace BetterGameLogic.Moves;


/// <summary>
/// A standard move, i.e. Not a Castle, Pawn Promotion or En Passant
/// </summary>
public class StandardMove : SinglePieceMove
{
    public override MoveType MoveType => MoveType.Standard;

    public StandardMove(Square from, Square to) 
        :base(from, to)
    {

    }

    

    public override void Apply(Board board)
    {
        IPiece? capturedPiece = board.At(To);
        ApplyWithoutUpdatingHistory(board);
        board.History.AddEntry(this, capturedPiece);
    }
    

    public override bool IsEquivalentTo(IMove move)
    {
        if (move.MoveType != MoveType)
        {
            return false;
        }

        StandardMove standardMove = (StandardMove)move;

        return standardMove.From == From &&
               standardMove.To == To;
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