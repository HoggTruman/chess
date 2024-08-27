using GameLogic.Enums;

namespace GameLogic.Moves;


/// <summary>
/// A standard move, i.e. Not a Castle, Pawn Promotion or En Passant
/// </summary>
public class StandardMove : SinglePieceMove
{
    public StandardMove(
        (int row, int col) from, 
        (int row, int col) to
    ) 
        :base(MoveType.Standard, from, to)
    {

    }
}