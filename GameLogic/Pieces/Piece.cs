using GameLogic.Enums;
using GameLogic.Interfaces;

namespace GameLogic.Pieces;

public abstract class Piece : IPiece
{
    #region Fields

    private int _row;
    private int _col;

    #endregion



    #region Properties

    public int Row
    {
        get { return _row; }
        set { _row = value; }
    }

    public int Col
    {
        get { return _col; }
        set { _col = value; }
    }

    public (int row, int col) Square
    {
        get { return (_row, _col); }
        set
        {
            _row = value.row;
            _col = value.col;
        }
    }

    public Color Color { get; }

    public PieceType PieceType { get; }

    public int Value { get; }

    #endregion



    #region Constructor

    protected Piece(int row, int col, Color color, PieceType pieceType, int value)
    {
        _row = row;
        _col = col;
        Color = color;
        PieceType = pieceType;
        Value = value;
    }

    #endregion



    #region Methods

    public abstract List<(int row, int col)> GetTargetedSquares(Board board);

    public abstract List<(int row, int col)> GetReachableSquares(Board board);

    #endregion
}