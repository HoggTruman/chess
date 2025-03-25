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


    public override List<Square> GetTargetedSquares()
    {
        return PieceHelpers.GetTargetedDiagonalSquares(Row, Col, _board);
    }

    public override List<Square> GetReachableSquares()
    {
        // Get targeted squares
        var squares = PieceHelpers.GetTargetedDiagonalSquares(Row, Col, _board);

        // Remove squares with a piece of the same color
        squares = squares
            .Where(s => 
                _board.State[s.Row, s.Col] == null || 
                _board.State[s.Row, s.Col]!.Color != Color)
            .ToList();

        return squares;
    }
}