using GameLogic.Enums;

namespace GameLogic.Moves;

public interface IMove
{
    MoveType MoveType { get; }
    Square From { get; }
    Square To { get; }
    Square Captured { get; }

    void Apply(Board board);
    bool LeavesPlayerInCheck(Board board);
    bool MovesSquare(Square square);
}
