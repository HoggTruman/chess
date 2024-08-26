using GameLogic.Enums;
using GameLogic.Interfaces;

namespace GameLogic.Moves;

public class PromotionMove : Move
{
    /// <summary>
    /// The type of piece the player chooses to promote to.
    /// Its value will be filled once the player makes the move.
    /// </summary>
    public PieceType? PromotedTo { get; set; }


    public PromotionMove(
        (int row, int col) from, 
        (int row, int col) to, 
        PieceType? promotedTo = null
    ) 
        :base(from, to, MoveType.Promotion)
    {
        PromotedTo = promotedTo;
    }
}