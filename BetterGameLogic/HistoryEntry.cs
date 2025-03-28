using BetterGameLogic.Moves;
using BetterGameLogic.Pieces;

namespace BetterGameLogic;

public class HistoryEntry(IMove move, IPiece? capturedPiece)
{
    public readonly IMove Move = move;
    public readonly IPiece? CapturedPiece = capturedPiece;
}
