using BetterGameLogic.Enums;

namespace BetterGameLogic.Moves;

/// <summary>
/// A Pawn Promotion move. Contains the PieceType promoted to 
/// </summary>
public class PromotionMove : SinglePieceMove
{
    public override MoveType MoveType => MoveType.Promotion;

    /// <summary>
    /// The type of piece the player chooses to promote to.
    /// Its value will be filled once the player makes the move.
    /// </summary>
    public PieceType PromotedTo { get; set; }


    public PromotionMove(Square from, Square to, PieceType promotedTo) 
        :base(from, to)
    {
        PromotedTo = promotedTo;
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

        PromotionMove promotionMove = (PromotionMove)move;

        return promotionMove.From == From &&
               promotionMove.To == To &&
               promotionMove.PromotedTo == PromotedTo;
    }    
}