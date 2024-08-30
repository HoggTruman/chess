using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Interfaces;
using GameLogic.Moves;

namespace GameLogic.Pieces;

public class KingPiece : Piece
{
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the KingPiece class.
    /// </summary>
    /// <param name="board">The Board object the piece will be placed on.</param>
    /// <param name="row">Row index from 0 to 7.</param>
    /// <param name="col">Column index from 0 to 7.</param>
    /// <param name="color">The Color of the piece.</param>
    public KingPiece(Board board, int row, int col, Color color)
        : base(board, row, col, color, PieceType.King, PieceValues.King)
    {

    }

    #endregion



    #region Public Methods

    public override List<(int row, int col)> GetTargetedSquares()
    {
        // Get all squares in range (including those out of bounds)
        List<(int row, int col)> targetedSquares = GetAllTargetedSquares();

        // Filter to keep only in bounds squares
        targetedSquares = targetedSquares.Where(BoardHelpers.SquareIsInBounds).ToList();

        return targetedSquares;
    }


    public override List<(int row, int col)> GetReachableSquares()
    {
        // Get all squares in range (including those out of bounds)
        List<(int row, int col)> squares = GetAllTargetedSquares();

        // Remove squares with a piece of the same color
        squares = squares
            .Where(s => _board.State[s.row, s.col]?.Color != Color)
            .ToList();

        return squares;
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
        var rooks = (List<RookPiece>)_board.Pieces[Color]
            .Where(x => x is RookPiece);

        foreach (var rook in rooks)
        {
            var castleSquares = GetCastleSquares(rook);

            if (castleSquares != null)
            {
                var kingTo = castleSquares.Value.kingTo;
                var rookTo = castleSquares.Value.rookTo; 
                validMoves.Add(
                    new CastleMove(Square, kingTo, rook.Square, rookTo)
                );
            }
        }

        return validMoves;
    }


    /// <summary>
    /// Returns a tuple containing the squares of the king and rook after castling if they can castle. Otherwise, returns null.
    /// </summary>
    /// <param name="rook">The RookPiece to castle with</param>
    /// <returns></returns>
    public ((int row, int col) kingTo, (int row, int col) rookTo)? GetCastleSquares(RookPiece rook)
    {
        if (rook.Color == Color && rook.CanCastle())
        {
            int kingDirection = Math.Sign(rook.Row - Row);

            (int row, int col) kingTo = (Row, Col + 2 * kingDirection);
            (int row, int col) rookTo = (Row, kingTo.col - kingDirection);

            return (kingTo, rookTo);
        }

        return null;
    }


    /// <summary>
    /// Determines if this KingPiece is under check.
    /// </summary>
    /// <returns>true if under check, otherwise false</returns>
    public bool IsChecked()
    {
        var enemyPieces = _board.Pieces[ColorHelpers.OppositeColor(Color)];

        return enemyPieces.Any(piece => piece.GetTargetedSquares().Contains(Square));
    }

    #endregion



    #region Private Methods

    /// <summary>
    /// Returns a List of all the squares the king could move to including those out of bounds.
    /// Does not include castling squares.
    /// </summary>
    /// <returns></returns>
    private List<(int row, int col)> GetAllTargetedSquares()
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

    #endregion
}