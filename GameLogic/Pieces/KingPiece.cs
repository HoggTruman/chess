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
    /// Constructor
    /// </summary>
    /// <param name="row">Row index from 0 to 7</param>
    /// <param name="col">Col index from 0 to 7</param>
    /// <param name="color"></param>
    public KingPiece(int row, int col, Color color=Color.White)
        : base(row, col, color, PieceType.King, PieceValues.King)
    {

    }

    #endregion



    #region Public Methods

    public override List<(int row, int col)> GetTargetedSquares(Board board)
    {
        // Get all squares in range (including those out of bounds)
        List<(int row, int col)> targetedSquares = GetAllTargetedSquares();

        // Filter to keep only in bounds squares
        targetedSquares = targetedSquares.Where(BoardHelpers.SquareIsInBounds).ToList();

        return targetedSquares;
    }


    public override List<(int row, int col)> GetReachableSquares(Board board)
    {
        // Get all squares in range (including those out of bounds)
        List<(int row, int col)> squares = GetAllTargetedSquares();

        // Remove squares with a piece of the same color
        squares = squares
            .Where(p => board.State[p.row, p.col]?.Color != Color)
            .ToList();

        return squares;
    }


    /// <summary>
    /// Returns a List of Moves including CastleMoves where possible.
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    public override List<IMove> GetValidMoves(Board board)
    {
        // Get all valid standard moves
        var validMoves = base.GetValidMoves(board);

        // Add valid castling moves
        var rooks = (List<RookPiece>)board.GetPiecesByColor(Color)
            .Where(x => x is RookPiece);

        foreach (var rook in rooks)
        {
            var castleSquares = GetCastleSquares(board, rook);

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
    /// <param name="board"></param>
    /// <param name="rook"></param>
    /// <returns></returns>
    public ((int row, int col) kingTo, (int row, int col) rookTo)? GetCastleSquares(Board board, RookPiece rook)
    {
        if (rook.Color == Color && rook.CanCastle(board))
        {
            int kingDirection = Math.Sign(rook.Row - Row);

            (int row, int col) kingTo = (Row, Col + 2 * kingDirection);
            (int row, int col) rookTo = (Row, kingTo.col - kingDirection);

            return (kingTo, rookTo);
        }

        return null;
    }


    public bool IsChecked(Board board)
    {
        var enemyPieces = board.GetPiecesByColor(ColorHelpers.OppositeColor(Color));

        return enemyPieces.Any(piece => piece.GetTargetedSquares(board).Contains(Square));
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