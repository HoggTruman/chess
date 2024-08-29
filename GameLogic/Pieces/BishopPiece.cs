using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;

namespace GameLogic.Pieces;

public class BishopPiece : Piece
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="board">The Board object the piece will be placed on</param>
    /// <param name="row">Row index from 0 to 7</param>
    /// <param name="col">Col index from 0 to 7</param>
    /// <param name="color"></param>
    public BishopPiece(Board board, int row, int col, Color color=Color.White) 
        : base(board, row, col, color, PieceType.Bishop, PieceValues.Bishop)
    {
    
    }


    public override List<(int row, int col)> GetTargetedSquares()
    {
        return PieceHelpers.GetTargetedDiagonalSquares(Row, Col, _board);
    }


    public override List<(int row, int col)> GetReachableSquares()
    {
        // Get targeted squares
        var squares = PieceHelpers.GetTargetedDiagonalSquares(Row, Col, _board);

        // Remove squares with a piece of the same color
        squares = squares
            .Where(s => _board.State[s.row, s.col]?.Color != Color)
            .ToList();

        return squares;
    }

}