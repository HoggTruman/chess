using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;

namespace GameLogic.Pieces;

public class RookPiece : Piece
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="row">Row index from 0 to 7</param>
    /// <param name="col">Col index from 0 to 7</param>
    /// <param name="color"></param>
    public RookPiece(int row, int col, Color color=Color.White)
        : base(row, col, color, PieceType.Rook, PieceValues.Rook)
    {

    }


    public override List<(int row, int col)> GetTargetedSquares(Board board)
    {
        return PieceHelpers.ScanRowAndCol(Row, Col, board);
    }


    public override List<(int row, int col)> GetReachableSquares(Board board)
    {
        // Get targeted squares
        List<(int row, int col)> squares = PieceHelpers.ScanRowAndCol(Row, Col, board);

        // Remove squares with a piece of the same color
        squares = squares
            .Where(p => board.State[p.row, p.col]?.Color != Color)
            .ToList();

        return squares;
    }


    /// <summary>
    /// Returns true if all conditions are met for castling with this piece.
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    public bool CanCastle(Board board)
    {
        // Ensure there is a king to castle with
        var king = board.Kings[Color];

        if (king == null)
            return false;

        // Ensure the rook and king have not moved
        if (HasMoved(board) || king.HasMoved(board))
            return false;

        // Ensure there are no pieces between the two
        List<(int row, int col)> betweenSquares = [];

        if (Col < king.Col)
            betweenSquares = [(Row, Col + 1), (Row, Col + 2), (Row, Col + 3)];
        
        if (Col > king.Col)
            betweenSquares = [(Row, Col - 1), (Row, Col - 2)];

        if (betweenSquares.Any(x => board.State[x.row, x.col] != null))
            return false;

        // Ensure the king does not pass through check
        List<(int row, int col)> kingSquares = [];

        if (Col < king.Col)
            kingSquares = [king.Square, (king.Row, king.Col - 1), (king.Row, king.Col - 2)];

        if (Col > king.Col)
            kingSquares = [king.Square, (king.Row, king.Col + 1), (king.Row, king.Col + 2)];
        
        var enemyPieces = board.GetPiecesByColor(ColorHelpers.OppositeColor(Color));

        foreach (var piece in enemyPieces)
        {
            if (piece.GetTargetedSquares(board).Intersect(kingSquares).Any())
                return false;
        }
        
        return true;
    }

}