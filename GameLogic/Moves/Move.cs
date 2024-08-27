using GameLogic.Enums;
using GameLogic.Interfaces;

namespace GameLogic.Moves;

public abstract class Move : IMove
{
    #region Properties

    public MoveType MoveType { get; }
    public (int row, int col) From { get; }
    public (int row, int col) To { get; }

    #endregion



    #region Constructor

    public Move(
        MoveType moveType,
        (int row, int col) from, 
        (int row, int col) to
    )
    {
        MoveType = moveType;
        From = from;
        To = to;
    }
    #endregion
}