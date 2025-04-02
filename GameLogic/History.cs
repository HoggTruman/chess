using GameLogic.Moves;
using GameLogic.Pieces;

namespace GameLogic;

public class History
{
    private readonly List<HistoryEntry> _entries = [];

    public void AddEntry(IMove move, IPiece? capturedPiece)
    {
        _entries.Add(new(move, capturedPiece));
    }

    public HistoryEntry? LastOrDefault() => _entries.LastOrDefault();

    public bool HasMoved(IPiece piece)
    {
        return _entries.Any(x => x.Move.MovesSquare(piece.StartSquare));
    }
}
