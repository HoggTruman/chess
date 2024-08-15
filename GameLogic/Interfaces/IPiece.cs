using GameLogic.Enums;

namespace GameLogic.Interfaces;

public interface IPiece
{
    // The row index of a piece from 0 to 7
    int Row { get; set; }

    // The column index of a piece from 0 to 7
    int Col { get; set; }

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
    /// <param name="board"></param>
    /// <returns>A list of (row, col) tuples</returns>
    List<(int row, int col)> GetTargetedSquares(Board board);

    /// <summary>
    /// Returns a list of valid moves for the piece
    /// </summary>
    /// <param name="board"></param>
    /// <returns>A list of (row, col) tuples</returns>
    List<(int row, int col)> GetValidMoves(Board board);


    //void Move(int row, int col);
    //void Take(IPiece target);
}