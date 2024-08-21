using GameLogic.Enums;
using GameLogic.Interfaces;

namespace GameLogic.Moves;

public class Move : IMove
{
    public MoveType MoveType { get; } = MoveType.Move;
    public (int row, int col) From { get; }
    public (int row, int col) To { get; }


    public Move((int row, int col) from, (int row, int col) to)
    {
        From = from;
        To = to;
    }
}