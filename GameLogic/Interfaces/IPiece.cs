using GameLogic.Enums;

namespace GameLogic.Interfaces;

public interface IPiece
{
    // The row index of a piece from 0 to 7
    int Row { get; set; }

    // The column index of a piece from 0 to 7
    int Col { get; set; }

    // A tuple with the current position of the piece
    (int row, int col) Square { get; set; }

    // A tuple with the starting square of the piece
    (int row, int col) StartSquare { get; }

    // The color of a piece (White or Black)
    Color Color { get; }

    // The type of a piece (e.g. Pawn, King, ...)
    PieceType PieceType { get; }

    // The point-value of a piece. e.g. 5 for a rook 
    int Value { get; }



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

}