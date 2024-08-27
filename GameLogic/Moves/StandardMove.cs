using GameLogic.Enums;

namespace GameLogic.Moves;

public class StandardMove : Move
{
    public StandardMove(
        (int row, int col) from, 
        (int row, int col) to
    ) 
        :base(MoveType.Standard, from, to)
    {

    }
}