using BetterGameLogic.Enums;

namespace BetterGameLogic.Moves;

public interface IMove
{
    MoveType MoveType { get; }
    Square From { get; }
    Square To { get; }

    void Apply(Board board);
    bool LeavesPlayerInCheck(Board board);

    bool MovesSquare(Square square);
    bool IsEquivalentTo(IMove move);
}
