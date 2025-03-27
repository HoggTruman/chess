using BetterGameLogic.Enums;
using BetterGameLogic.Pieces;

namespace BetterGameLogic.Moves;

public interface IMove
{
    MoveType MoveType { get; }
    Square From { get; }
    Square To { get; }

    void Apply(Board board);
    void Undo(Board board, IPiece? capturedPiece);
    bool LeavesPlayerInCheck(Board board);

    bool MovesSquare(Square square);
    bool IsEquivalentTo(IMove move);
}
