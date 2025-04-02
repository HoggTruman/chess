using GameLogic.Moves;
using GameLogic.Pieces;

namespace GameLogic;

public class HistoryEntry(IMove move, IPiece? capturedPiece)
{
    public readonly IMove Move = move;
    public readonly IPiece? CapturedPiece = capturedPiece;
}
