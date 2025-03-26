using BetterGameLogic.Constants;
using BetterGameLogic.Enums;

namespace BetterGameLogic.Pieces;

public class KnightPiece : Piece
{
    public override PieceType PieceType => PieceType.Knight;
    public override int Value => PieceValues.Knight;

    public KnightPiece(Board board, int row, int col, PieceColor color)
        : base(board, row, col, color)
    {

    }


    public override List<Square> GetTargetedSquares()
    {
        // Get all squares in range (including those out of bounds)
        var targetedSquares = GetAllTargetedSquares();
        targetedSquares = targetedSquares.Where(Board.IsInBounds);

        return targetedSquares.ToList();
    }

    public override List<Square> GetReachableSquares()
    {
        // Get all squares in range (including those out of bounds)
        var squares = GetAllTargetedSquares();
        squares = squares.Where(Board.IsInBounds);

        // Remove squares with a piece of the same color
        squares = squares.Where(s => 
            _board.State[s.Row, s.Col] == null ||
            _board.State[s.Row, s.Col]!.Color != Color);

        return squares.ToList();
    }


    private IEnumerable<Square> GetAllTargetedSquares()
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
}