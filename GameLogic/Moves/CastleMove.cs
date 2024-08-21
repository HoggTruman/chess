using GameLogic.Enums;
using GameLogic.Interfaces;

namespace GameLogic.Moves;

public class CastleMove : IMove
{
    public MoveType MoveType { get; } = MoveType.Castle;

    // From and To squares of the king
    public (int row, int col) From { get; }
    public (int row, int col) To { get; }

    // From and To squares of the rook
    public (int row, int col) RookFrom { get; }
    public (int row, int col) RookTo { get; }


    public CastleMove((int row, int col) from, (int row, int col) to, (int row, int col) rookFrom, (int row, int col) rookTo)
    {
        From = from;
        To = to;
        RookFrom = rookFrom;
        RookTo = rookTo;
    }
}