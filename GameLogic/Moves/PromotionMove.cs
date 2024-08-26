using GameLogic.Enums;
using GameLogic.Interfaces;

namespace GameLogic.Moves;

public class PromotionMove : Move
{
    public PieceType PromotedTo { get; }
    public IPiece PromotionPiece { get; }


    public PromotionMove(
        (int row, int col) from, 
        (int row, int col) to, 
        PieceType promotedTo,
        IPiece promotionPiece
    ) 
        :base(MoveType.Promotion, from, to)
    {
        PromotedTo = promotedTo;
        PromotionPiece = promotionPiece;
    }
}