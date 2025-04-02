using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;

namespace GameLogic.Pieces;

public class RookPiece : Piece
{
    public override PieceType PieceType => PieceType.Rook;
    public override int Value => PieceValues.Rook;

    public RookPiece(Board board, int row, int col, PieceColor color, Square? startSquare = null)
        : base(board, row, col, color, startSquare)
    {

    }

    public RookPiece(Board board, Square square, PieceColor color, Square? startSquare = null) 
        : base(board, square.Row, square.Col, color, startSquare)
    {
        
    }


    public override List<Square> GetTargetedSquares()
    {
        return PieceHelpers.GetTargetedRowColSquares(Row, Col, _board);
    }

    public override List<Square> GetReachableSquares()
    {
        var squares = PieceHelpers.GetTargetedRowColSquares(Row, Col, _board)
            .Where(s => !_board.IsOccupiedByColor(s, Color)).ToList();

        return squares;
    }


    /// <summary>
    /// Returns true if all conditions are met for castling with this piece.
    /// </summary>
    /// <returns></returns>
    public bool CanCastle()
    {
        // Ensure there is a king to castle with
        var king = _board.GetKing(Color);

        if (king == null ||
            HasMoved() || 
            king.HasMoved() ||
            Row != king.Row ||
            Col == king.Col)
        {
            return false;
        }

        // Ensure the rook's start square is a rook start square
        if (StartSquare != StartSquares.WhiteRookK &&
            StartSquare != StartSquares.WhiteRookQ &&
            StartSquare != StartSquares.BlackRookK &&
            StartSquare != StartSquares.BlackRookQ)
        {
            return false;
        }

        // Ensure there are no pieces between the two
        List<Square> betweenSquares = Col < king.Col?
            [new(Row, Col + 1), new(Row, Col + 2), new(Row, Col + 3)]:
            [new(Row, Col - 1), new(Row, Col - 2)];

        if (betweenSquares.Any(s => _board.State[s.Row, s.Col] != null))
        {
            return false;
        }            

        // Ensure the king does not pass through check
        List<Square> kingSquares = Col < king.Col?
            [king.Square, new(king.Row, king.Col - 1), new(king.Row, king.Col - 2)]:
            [king.Square, new(king.Row, king.Col + 1), new(king.Row, king.Col + 2)];
        
        var enemyPieces = _board.Pieces[ColorHelpers.Opposite(Color)];

        foreach (var piece in enemyPieces)
        {
            if (piece.GetTargetedSquares().Intersect(kingSquares).Any())
                return false;
        }
        
        return true;
    }
}