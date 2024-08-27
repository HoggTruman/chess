using GameLogic.Enums;

namespace GameLogic.Moves;

/// <summary>
/// Holds data for a Castle Move.
/// From and To refer to the position of the king
/// </summary>
public class CastleMove : Move
{
    public (int row, int col) RookFrom { get; }
    public (int row, int col) RookTo { get; }

    public CastleMove(
        (int row, int col) kingFrom,
        (int row, int col) kingTo,
        (int row, int col) rookFrom,
        (int row, int col) rookTo
    ) 
        :base(kingFrom, kingTo, MoveType.Castle)
    {
        RookFrom = rookFrom;
        RookTo = rookTo;
    }
}