using GameLogic.Enums;

namespace GameLogic.Interfaces;

public interface IPiece
{
    #region Properties

    /// <summary>
    /// The row index of a piece from 0 to 7
    /// </summary>
    int Row { get; set; }

    /// <summary>
    /// The column index of a piece from 0 to 7
    /// </summary>
    int Col { get; set; }

    /// <summary>
    /// A (row, col) tuple with the current position of the piece
    /// </summary>
    (int row, int col) Square { get; set; }

    /// <summary>
    /// A (row, col) tuple with the starting square of the piece
    /// </summary>
    (int row, int col) StartSquare { get; }

    /// <summary>
    /// The color of a piece (White or Black)
    /// </summary>
    Color Color { get; }

    /// <summary>
    /// The type of a piece (e.g. Pawn, King, ...)
    /// </summary>
    PieceType PieceType { get; }

    /// <summary>
    /// The point-value of a piece. e.g. 5 for a rook 
    /// </summary>
    int Value { get; }

    #endregion



    #region Methods

    /// <summary>
    /// Returns a list of the squares which the piece threatens with a capture.
    /// Whether or not these squares are valid moves is not considered.
    /// </summary>
    /// <returns>A list of (row, col) tuples</returns>
    List<(int row, int col)> GetTargetedSquares();


    /// <summary>
    /// Returns a list of the squares which the piece can reach in a single move.
    /// Whether or not these squares are valid moves is not considered.
    /// </summary>
    /// <returns>A list of (row, col) tuples</returns>
    List<(int row, int col)> GetReachableSquares();


    /// <summary>
    /// Returns a list of moves which are valid for a player to make.
    /// </summary>
    /// <returns></returns>
    List<IMove> GetValidMoves();


    /// <summary>
    /// Returns a bool of whether the piece has moved or not.
    /// </summary>
    /// <returns></returns>
    bool HasMoved();

    #endregion
}