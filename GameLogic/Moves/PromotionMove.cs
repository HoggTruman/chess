using GameLogic.Enums;
using GameLogic.Interfaces;
using System.Text.RegularExpressions;

namespace GameLogic.Moves;

/// <summary>
/// A Pawn Promotion move. Contains the PieceType promoted to 
/// </summary>
public class PromotionMove : SinglePieceMove
{
    /// <summary>
    /// The type of piece the player chooses to promote to.
    /// Its value will be filled once the player makes the move.
    /// </summary>
    public PieceType PromotedTo { get; set; }


    public PromotionMove(
        (int row, int col) from, 
        (int row, int col) to, 
        PieceType promotedTo = PieceType.Pawn
    ) 
        :base(MoveType.Promotion, from, to)
    {
        PromotedTo = promotedTo;
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