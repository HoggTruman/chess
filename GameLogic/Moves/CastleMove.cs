using GameLogic.Enums;
using GameLogic.Interfaces;

namespace GameLogic.Moves;

/// <summary>
/// A Castle move. Describes movement of the King and Rook
/// </summary>
public class CastleMove : IMove
{
    #region Properties

    public MoveType MoveType { get; } = MoveType.Castle;

    /// <summary>
    /// The square the king moves from.
    /// </summary>
    public (int row, int col) From { get; }

    /// <summary>
    /// The square the king moves to.
    /// </summary>
    public (int row, int col) To { get; }

    /// <summary>
    /// The square the rook moves from.
    /// </summary>
    public (int row, int col) RookFrom { get; }

    /// <summary>
    /// The square the rook moves to.
    /// </summary>
    public (int row, int col) RookTo { get; }

    #endregion



    #region Constructor

    public CastleMove(
        (int row, int col) from,
        (int row, int col) to,
        (int row, int col) rookFrom,
        (int row, int col) rookTo
    ) 
    {
        From = from;
        To = to;
        RookFrom = rookFrom;
        RookTo = rookTo;
    }

    #endregion



    #region Methods

    public bool MovesSquare((int row, int col) square)
    {
        return square == From || square == RookFrom;
    }

    #endregion
}