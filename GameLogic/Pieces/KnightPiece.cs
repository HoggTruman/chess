using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;

namespace GameLogic.Pieces;

public class KnightPiece : Piece
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the KnightPiece class.
    /// </summary>
    /// <param name="board">The Board object the piece will be placed on.</param>
    /// <param name="row">Row index from 0 to 7.</param>
    /// <param name="col">Column index from 0 to 7.</param>
    /// <param name="color">The Color of the piece.</param>
    public KnightPiece(Board board, int row, int col, Color color)
        : base(board, row, col, color, PieceType.Knight, PieceValues.Knight)
    {

    }

    #endregion



    #region Public Methods

    public override List<(int row, int col)> GetTargetedSquares()
    {
        // Get all squares in range (including those out of bounds)
        List<(int row, int col)> targetedSquares = GetAllTargetedSquares();

        // Filter to keep only in bounds squares
        targetedSquares = targetedSquares.Where(BoardHelpers.SquareIsInBounds).ToList();

        return targetedSquares;
    }


    public override List<(int row, int col)> GetReachableSquares()
    {
        // Get all squares in range (including those out of bounds)
        List<(int row, int col)> squares = GetAllTargetedSquares();

        // Remove squares with a piece of the same color
        squares = squares
            .Where(s => 
                _board.State[s.row, s.col] == null ||
                _board.State[s.row, s.col]?.Color != Color)
            .ToList();

        return squares;
    }

    #endregion



    #region Private Methods

    private List<(int row, int col)> GetAllTargetedSquares()
    {
        return [
            new(Row - 2, Col - 1),
            new(Row - 2, Col + 1),
            new(Row + 2, Col - 1),
            new(Row + 2, Col + 1),
            new(Row - 1, Col - 2),
            new(Row + 1, Col - 2),
            new(Row - 1, Col + 2),
            new(Row + 1, Col + 2)
        ];
    }

    #endregion

}