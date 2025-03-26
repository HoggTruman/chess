using BetterGameLogic.Enums;

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
        throw new NotImplementedException();
    }

    public override void Undo(Board board)
    {
        throw new NotImplementedException();
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
}