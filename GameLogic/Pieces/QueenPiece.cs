using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;

namespace GameLogic.Pieces;

public class QueenPiece : Piece
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="row">Row index from 0 to 7</param>
    /// <param name="col">Col index from 0 to 7</param>
    /// <param name="color"></param>
    public QueenPiece(int row, int col, Color color=Color.White)
        : base(row, col, color, PieceType.Queen, PieceValues.Queen)
    {

    }

    public override List<(int row, int col)> GetTargetedSquares(Board board)
    {
        List<(int row, int col)> targetedSquares = [];

        // Get targeted squares on the piece's row and column
        targetedSquares.AddRange(PieceHelpers.ScanRowAndCol(Row, Col, board));

        // Get targeted squares on the piece's diagonals
        targetedSquares.AddRange(PieceHelpers.ScanDiagonals(Row, Col, board));

        return targetedSquares;
    }


    public override List<(int row, int col)> GetReachableSquares(Board board)
    {
        List<(int row, int col)> reachableSquares = [];

        // Get targeted squares on the piece's row and column
        reachableSquares.AddRange(PieceHelpers.ScanRowAndCol(Row, Col, board));

        // Get targeted squares on the piece's diagonals
        reachableSquares.AddRange(PieceHelpers.ScanDiagonals(Row, Col, board));

        // Remove squares with a piece of the same color as the moving piece
        reachableSquares = reachableSquares
            .Where(p => board.State[p.row, p.col]?.Color != Color)
            .ToList();

        return reachableSquares;
    }


}