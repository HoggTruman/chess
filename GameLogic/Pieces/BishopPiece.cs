using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;

namespace GameLogic.Pieces;

public class BishopPiece : Piece
{
    public override PieceType PieceType => PieceType.Bishop;
    public override int Value => PieceValues.Bishop;

    public BishopPiece(Board board, int row, int col, PieceColor color, Square? startSquare = null) 
        : base(board, row, col, color, startSquare)
    {
    
    }

    public BishopPiece(Board board, Square square, PieceColor color, Square? startSquare = null) 
        : base(board, square.Row, square.Col, color, startSquare)
    {
        
    }


    public override List<Square> GetTargetedSquares()
    {
        return PieceHelpers.GetTargetedDiagonalSquares(Row, Col, _board);
    }

    public override List<Square> GetReachableSquares()
    {
        var squares = PieceHelpers.GetTargetedDiagonalSquares(Row, Col, _board)
            .Where(s => !_board.IsOccupiedByColor(s, Color))
            .ToList();

        return squares;
    }
}