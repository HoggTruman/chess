using GameLogic.Enums;
using GameLogic.Interfaces;

namespace GameLogic.Moves;

/// <summary>
/// An abstract class for moves that only move a single piece (any move other than castling)
/// </summary>
public abstract class SinglePieceMove : IMove
{
    #region Properties

    public MoveType MoveType { get; }
    public (int row, int col) From { get; }
    public (int row, int col) To { get; }

    #endregion



    #region Constructor

    public SinglePieceMove(
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



    #region Methods

    public bool MovesSquare((int row, int col) square)
    {
        return square == From;
    }

    #endregion
}