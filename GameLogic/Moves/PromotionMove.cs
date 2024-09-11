using GameLogic.Enums;

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
}