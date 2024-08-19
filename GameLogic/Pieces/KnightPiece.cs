using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;

namespace GameLogic.Pieces;

public class KnightPiece : Piece
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="row">Row index from 0 to 7</param>
    /// <param name="col">Col index from 0 to 7</param>
    /// <param name="color"></param>
    /// <param name="value">The value of the piece. Defaults to knight value but can be set manually for pawn promotion</param>
    public KnightPiece(int row, int col, Color color=Color.White, int value=PieceValues.Knight)
        : base(row, col, color, PieceType.Knight, value)
    {

    }


    public override List<(int row, int col)> GetTargetedSquares(Board board)
    {
        // Get all squares in range (including those out of bounds)
        List<(int row, int col)> targetedSquares = [
            new(Row - 2, Col - 1),
            new(Row - 2, Col + 1),
            new(Row + 2, Col - 1),
            new(Row + 2, Col + 1),
            new(Row - 1, Col - 2),
            new(Row + 1, Col - 2),
            new(Row - 1, Col + 2),
            new(Row + 1, Col + 2)
        ];

        // Filter to keep only in bounds squares
        targetedSquares = BoardHelpers.KeepInBoundsSquares(targetedSquares);

        return targetedSquares;
    }

    public override List<(int row, int col)> GetReachableSquares(Board board)
    {
        // Get all squares in range (including those out of bounds)
        List<(int row, int col)> squares = [
            new(Row - 2, Col - 1),
            new(Row - 2, Col + 1),
            new(Row + 2, Col - 1),
            new(Row + 2, Col + 1),
            new(Row - 1, Col - 2),
            new(Row + 1, Col - 2),
            new(Row - 1, Col + 2),
            new(Row + 1, Col + 2)
        ];

        // Remove squares with a piece of the same color
        squares = squares
            .Where(p => 
                board.State[p.row, p.col] == null ||
                board.State[p.row, p.col]?.Color != Color)
            .ToList();

        return squares;
    }

}