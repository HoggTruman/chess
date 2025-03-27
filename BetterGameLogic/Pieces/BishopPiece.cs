using BetterGameLogic.Constants;
using BetterGameLogic.Enums;
using BetterGameLogic.Helpers;

namespace BetterGameLogic.Pieces;

public class BishopPiece : Piece
{
    public override PieceType PieceType => PieceType.Bishop;
    public override int Value => PieceValues.Bishop;

    public BishopPiece(Board board, int row, int col, PieceColor color) 
        : base(board, row, col, color)
    {
    
    }

    public BishopPiece(Board board, Square square, PieceColor color) 
        : this(board, square.Row, square.Col, color)
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