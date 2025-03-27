using BetterGameLogic.Constants;
using BetterGameLogic.Enums;
using BetterGameLogic.Helpers;

namespace BetterGameLogic.Pieces;

public class QueenPiece : Piece
{
    public override PieceType PieceType => PieceType.Queen;
    public override int Value => PieceValues.Queen;

    public QueenPiece(Board board, int row, int col, PieceColor color)
        : base(board, row, col, color)
    {

    }

    public QueenPiece(Board board, Square square, PieceColor color) 
        : this(board, square.Row, square.Col, color)
    {
        
    }
    

    public override List<Square> GetTargetedSquares()
    {
        List<Square> targetedSquares = [];
        targetedSquares.AddRange(PieceHelpers.GetTargetedRowColSquares(Row, Col, _board));
        targetedSquares.AddRange(PieceHelpers.GetTargetedDiagonalSquares(Row, Col, _board));

        return targetedSquares;
    }


    public override List<Square> GetReachableSquares()
    {
        var reachableSquares = PieceHelpers.GetTargetedRowColSquares(Row, Col, _board)
            .Concat(PieceHelpers.GetTargetedDiagonalSquares(Row, Col, _board))
            .Where(s => !_board.IsOccupiedByColor(s, Color))
            .ToList();

        return reachableSquares;
    }
}