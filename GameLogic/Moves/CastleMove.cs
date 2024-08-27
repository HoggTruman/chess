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
    public (int row, int col) KingFrom { get; }
    public (int row, int col) KingTo { get; }
    public (int row, int col) RookFrom { get; }
    public (int row, int col) RookTo { get; }

    #endregion



    #region Constructor

    public CastleMove(
        (int row, int col) kingFrom,
        (int row, int col) kingTo,
        (int row, int col) rookFrom,
        (int row, int col) rookTo
    ) 
    {
        KingFrom = kingFrom;
        KingTo = kingTo;
        RookFrom = rookFrom;
        RookTo = rookTo;
    }

    #endregion



    #region Methods

    public bool MovesSquare((int row, int col) square)
    {
        return square == KingFrom || square == RookFrom;
    }

    #endregion
}