using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;

namespace GameLogic.Pieces;

public class BishopPiece : Piece
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="row">Row index from 0 to 7</param>
    /// <param name="col">Col index from 0 to 7</param>
    /// <param name="color"></param>
    /// <param name="value">The value of the piece. Defaults to bishop value but can be set manually for pawn promotion</param>
    public BishopPiece(int row, int col, Color color=Color.White, int value=PieceValues.Bishop) 
        : base(row, col, color, PieceType.Bishop, value)
    {
    
    }

    public override List<(int row, int col)> GetTargetedSquares(Board board)
    {
        return PieceHelpers.ScanDiagonals(Row, Col, board);
    }

    public override List<(int row, int col)> GetReachableSquares(Board board)
    {
        throw new NotImplementedException();
    }

}