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

    public KnightPiece(Board board, Square square, PieceColor color) 
        : this(board, square.Row, square.Col, color)
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
        var squares = GetAllTargetedSquares()
            .Where(Board.IsInBounds)
            .Where(s => !_board.IsOccupiedByColor(s, Color));

        return squares.ToList();
    }

    /// <summary>
    /// Retrieves all targeted squares including those out of bounds
    /// </summary>
    /// <returns></returns>
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