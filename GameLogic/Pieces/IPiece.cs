using GameLogic.Enums;
using GameLogic.Moves;

namespace GameLogic.Pieces;

public interface IPiece
{
    int Row { get; set; }
    int Col { get; set; }
    Square Square { get; set; }
    Square StartSquare { get; }
    PieceColor Color { get; }
    PieceType PieceType { get; }
    int Value { get; }

    /// <summary>
    /// Returns a list of the squares which the piece threatens with a capture.
    /// Whether or not these squares are valid moves is not considered.
    /// </summary>
    /// <returns>A list of Squares</returns>
    List<Square> GetTargetedSquares();


    /// <summary>
    /// Returns a list of the squares which the piece can reach in a single move.
    /// Whether or not these squares are valid moves is not considered.
    /// </summary>
    /// <returns>A list of Squares</returns>
    List<Square> GetReachableSquares();


    /// <summary>
    /// Returns a list of moves which are valid for the piece to make.
    /// </summary>
    /// <returns></returns>
    List<IMove> GetValidMoves();


    /// <summary>
    /// Returns a bool of whether the piece has moved or not.
    /// </summary>
    /// <returns></returns>
    bool HasMoved();
}
