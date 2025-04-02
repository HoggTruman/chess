using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Moves;

namespace GameLogic.Pieces;

public class KingPiece : Piece
{
    public override PieceType PieceType => PieceType.King;
    public override int Value => PieceValues.King;


    public KingPiece(Board board, int row, int col, PieceColor color)
        : base(board, row, col, color)
    {

    }

    public KingPiece(Board board, Square square, PieceColor color) 
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
        // Get all squares in range (including those out of bounds)
        var squares = GetAllTargetedSquares();
        squares = squares.Where(Board.IsInBounds);
        squares = squares.Where(s => !_board.IsOccupiedByColor(s, Color));
            
        return squares.ToList();
    }


    /// <summary>
    /// Returns a List of Moves including CastleMoves where possible.
    /// </summary>
    /// <returns></returns>
    public override List<IMove> GetValidMoves()
    {
        // Get all valid standard moves
        var validMoves = base.GetValidMoves();

        // Add valid castling moves
        foreach (var piece in _board.Pieces[Color])
        {
            if (piece is RookPiece == false)
            {
                continue;
            }

            RookPiece rook = (RookPiece)piece;

            var castleSquares = GetCastleSquares(rook);

            if (castleSquares != null)
            {
                var kingTo = castleSquares.Value.kingTo;
                var rookTo = castleSquares.Value.rookTo; 
                validMoves.Add(new CastleMove(Square, kingTo, rook.Square, rookTo));
            }
        }

        return validMoves;
    }


    /// <summary>
    /// Returns a tuple containing the squares of the king and rook after castling if they can castle. Otherwise, returns null.
    /// </summary>
    /// <param name="rook">The RookPiece to castle with</param>
    /// <returns></returns>
    public (Square kingTo, Square rookTo)? GetCastleSquares(RookPiece rook)
    {
        if (rook.Color == Color && rook.CanCastle())
        {
            int kingDirection = Math.Sign(rook.Col - Col);
            Square kingTo = new(Row, Col + 2 * kingDirection);
            Square rookTo = new(Row, kingTo.Col - kingDirection);

            return (kingTo, rookTo);
        }

        return null;
    }


    /// <summary>
    /// Determines if this KingPiece is under check.
    /// </summary>
    /// <returns>true if under check, otherwise false</returns>
    public bool IsUnderCheck()
    {
        var enemyPieces = _board.Pieces[ColorHelpers.Opposite(Color)];
        return enemyPieces.Any(piece => piece.GetTargetedSquares().Contains(Square));
    }


    /// <summary>
    /// Returns a List of all the squares the king could move to including those out of bounds.
    /// Does not include castling squares.
    /// </summary>
    /// <returns></returns>
    private IEnumerable<Square> GetAllTargetedSquares()
    {
        return [
            new(Row - 1, Col),
            new(Row + 1, Col),
            new(Row, Col - 1),
            new(Row, Col + 1),
            new(Row - 1, Col - 1),
            new(Row - 1, Col + 1),
            new(Row + 1, Col - 1),
            new(Row + 1, Col + 1)
        ];
    }
}