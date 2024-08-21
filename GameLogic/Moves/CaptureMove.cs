using GameLogic.Enums;
using GameLogic.Interfaces;

namespace GameLogic.Moves;

public class CaptureMove : IMove
{
    public MoveType MoveType { get; } = MoveType.Capture;
    public (int row, int col) From { get; }
    public (int row, int col) To { get; }
    public (int row, int col) Captured { get; }


    public CaptureMove((int row, int col) from, (int row, int col) to, (int row, int col) captured)
    {
        From = from;
        To = to;
        Captured = captured;
    }
}