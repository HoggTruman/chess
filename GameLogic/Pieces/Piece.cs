using GameLogic.Enums;
using GameLogic.Interfaces;
using GameLogic.Moves;

namespace GameLogic.Pieces;

public abstract class Piece : IPiece
{
    #region Fields

    protected readonly Board _board;
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

    public (int row, int col) StartSquare { get; }

    public Color Color { get; }

    public PieceType PieceType { get; }

    public int Value { get; }

    #endregion



    #region Constructor

    /// <summary>
    /// Base constructor for the abstract Piece Class
    /// </summary>
    /// <param name="board">The Board object the piece will be placed on.</param>
    /// <param name="row">Row index from 0 to 7.</param>
    /// <param name="col">Column index from 0 to 7.</param>
    /// <param name="color">The Color of the piece.</param>
    /// <param name="pieceType">The type of a piece. (e.g. Pawn, King, ...)</param>
    /// <param name="value">The point-value of a piece. e.g. 5 for a rook</param>
    protected Piece(Board board, int row, int col, Color color, PieceType pieceType, int value)
    {
        _board = board;
        _row = row;
        _col = col;
        StartSquare = (row, col);
        Color = color;
        PieceType = pieceType;
        Value = value;
    }

    #endregion



    #region Methods

    public abstract List<(int row, int col)> GetTargetedSquares();


    public abstract List<(int row, int col)> GetReachableSquares();


    public virtual List<IMove> GetValidMoves()
    {
        List<IMove> validMoves = [];

        // keep the squares which represent a valid move
        foreach (var toSquare in GetReachableSquares())
        {
            if (_board.MoveLeavesPlayerInCheck(Square, toSquare) == false)
            {
                validMoves.Add(
                    new StandardMove(Square, toSquare)
                );
            }
        }

        return validMoves;
    }


    public bool HasMoved()
    {
        return _board.MoveHistory.Any(move => move.MovesSquare(StartSquare));
    }

    #endregion
}