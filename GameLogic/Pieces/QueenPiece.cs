using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;

namespace GameLogic.Pieces;

public class QueenPiece : Piece
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="board">The Board object the piece will be placed on</param>
    /// <param name="row">Row index from 0 to 7</param>
    /// <param name="col">Col index from 0 to 7</param>
    /// <param name="color"></param>
    public QueenPiece(Board board, int row, int col, Color color=Color.White)
        : base(board, row, col, color, PieceType.Queen, PieceValues.Queen)
    {

    }
    

    public override List<(int row, int col)> GetTargetedSquares()
    {
        List<(int row, int col)> targetedSquares = [];

        // Get targeted squares on the piece's row and column
        targetedSquares.AddRange(PieceHelpers.GetTargetedRowColSquares(Row, Col, _board));

        // Get targeted squares on the piece's diagonals
        targetedSquares.AddRange(PieceHelpers.GetTargetedDiagonalSquares(Row, Col, _board));

        return targetedSquares;
    }


    public override List<(int row, int col)> GetReachableSquares()
    {
        List<(int row, int col)> reachableSquares = [];

        // Get targeted squares on the piece's row and column
        reachableSquares.AddRange(PieceHelpers.GetTargetedRowColSquares(Row, Col, _board));

        // Get targeted squares on the piece's diagonals
        reachableSquares.AddRange(PieceHelpers.GetTargetedDiagonalSquares(Row, Col, _board));

        // Remove squares with a piece of the same color as the moving piece
        reachableSquares = reachableSquares
            .Where(s => _board.State[s.row, s.col]?.Color != Color)
            .ToList();

        return reachableSquares;
    }


}