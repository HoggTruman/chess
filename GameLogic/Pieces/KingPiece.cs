using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;

namespace GameLogic.Pieces;

public class KingPiece : Piece
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="row">Row index from 0 to 7</param>
    /// <param name="col">Col index from 0 to 7</param>
    /// <param name="color"></param>
    public KingPiece(int row, int col, Color color=Color.White)
        : base(row, col, color, PieceType.King, PieceValues.King)
    {

    }

    #endregion



    #region Public Methods

    public override List<(int row, int col)> GetTargetedSquares(Board board)
    {
        // Get all squares in range (including those out of bounds)
        List<(int row, int col)> targetedSquares = GetAllTargetedSquares();

        // Filter to keep only in bounds squares
        targetedSquares = targetedSquares.Where(BoardHelpers.SquareIsInBounds).ToList();

        return targetedSquares;
    }


    public override List<(int row, int col)> GetReachableSquares(Board board)
    {
        // Get all squares in range (including those out of bounds)
        List<(int row, int col)> squares = GetAllTargetedSquares();

        // Remove squares with a piece of the same color
        squares = squares
            .Where(p => board.State[p.row, p.col]?.Color != Color)
            .ToList();

        return squares;
    }


    public bool IsChecked(Board board)
    {
        var enemyPieces = board.GetPiecesByColor(ColorHelpers.OppositeColor(Color));

        return enemyPieces.Any(piece => piece.GetTargetedSquares(board).Contains(Square));
    }

    #endregion



    #region Private Methods

    /// <summary>
    /// Returns a List of all the squares the king could move to including those out of bounds.
    /// Does not include castling squares.
    /// </summary>
    /// <returns></returns>
    private List<(int row, int col)> GetAllTargetedSquares()
    {
        return [
            new(Row - 1, Col),
            new(Row + 1, Col),
            new(Row, Col - 1),
            new(Row, Col + 1),
            new(Row - 1, Col - 1),
            new(Row - 1, Col + 1),
            new(Row + 1, Col - 1),
            new(Row + 1, Col + 1)
        ];
    }

    #endregion
}