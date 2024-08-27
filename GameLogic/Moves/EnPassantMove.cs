using GameLogic.Enums;

namespace GameLogic.Moves;

public class EnPassantMove : Move
{
    public (int row, int col) Captured { get; }

    public EnPassantMove(
        (int row, int col) from, 
        (int row, int col) to,
        (int row, int col) captured
    ) 
        :base(from, to, MoveType.EnPassant)
    {
        Captured = captured;
    }
}