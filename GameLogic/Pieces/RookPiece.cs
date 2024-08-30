using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;

namespace GameLogic.Pieces;

public class RookPiece : Piece
{
    /// <summary>
    /// Initializes a new instance of the RookPiece class.
    /// </summary>
    /// <param name="board">The Board object the piece will be placed on.</param>
    /// <param name="row">Row index from 0 to 7.</param>
    /// <param name="col">Column index from 0 to 7.</param>
    /// <param name="color">The Color of the piece.</param>
    public RookPiece(Board board, int row, int col, Color color)
        : base(board, row, col, color, PieceType.Rook, PieceValues.Rook)
    {

    }


    public override List<(int row, int col)> GetTargetedSquares()
    {
        return PieceHelpers.GetTargetedRowColSquares(Row, Col, _board);
    }


    public override List<(int row, int col)> GetReachableSquares()
    {
        // Get targeted squares
        var squares = PieceHelpers.GetTargetedRowColSquares(Row, Col, _board);

        // Remove squares with a piece of the same color
        squares = squares
            .Where(s => _board.State[s.row, s.col]?.Color != Color)
            .ToList();

        return squares;
    }


    /// <summary>
    /// Returns true if all conditions are met for castling with this piece.
    /// </summary>
    /// <returns></returns>
    public bool CanCastle()
    {
        // Ensure there is a king to castle with
        var king = _board.Kings[Color];

        if (king == null)
            return false;

        // Ensure the rook and king have not moved
        if (HasMoved() || king.HasMoved())
            return false;

        // Ensure there are no pieces between the two
        List<(int row, int col)> betweenSquares = [];

        if (Col < king.Col)
            betweenSquares = [(Row, Col + 1), (Row, Col + 2), (Row, Col + 3)];
        
        if (Col > king.Col)
            betweenSquares = [(Row, Col - 1), (Row, Col - 2)];

        if (betweenSquares.Any(s => _board.State[s.row, s.col] != null))
            return false;

        // Ensure the king does not pass through check
        List<(int row, int col)> kingSquares = [];

        if (Col < king.Col)
            kingSquares = [king.Square, (king.Row, king.Col - 1), (king.Row, king.Col - 2)];

        if (Col > king.Col)
            kingSquares = [king.Square, (king.Row, king.Col + 1), (king.Row, king.Col + 2)];
        
        var enemyPieces = _board.Pieces[ColorHelpers.OppositeColor(Color)];

        foreach (var piece in enemyPieces)
        {
            if (piece.GetTargetedSquares().Intersect(kingSquares).Any())
                return false;
        }
        
        return true;
    }

}