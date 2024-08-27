using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Interfaces;
using GameLogic.Moves;

namespace GameLogic.Pieces;

public class PawnPiece : Piece
{
    // The forward direction for the piece (+1 for White, -1 for Black)
    private readonly int _fwd;


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="row">Row index from 0 to 7</param>
    /// <param name="col">Col index from 0 to 7</param>
    /// <param name="color"></param>
    public PawnPiece(int row, int col, Color color=Color.White)
        : base(row, col, color, PieceType.Pawn, PieceValues.Pawn)
    {
        _fwd = color == Color.White? 1: -1;
    }


    /// <summary>
    /// Returns the targeted squares diagonally in front of a pawn.
    /// En Passant is not considered here
    /// </summary>
    /// <param name="board"></param>
    /// <returns>A list of (row, col) tuples</returns>
    public override List<(int row, int col)> GetTargetedSquares(Board board)
    {
        List<(int row, int col)> targetedSquares = [];

        // Add targeted squares (if in bounds)
        if (BoardHelpers.SquareIsInBounds((Row + _fwd, Col - 1)))
            targetedSquares.Add((Row + _fwd, Col - 1));
        
        if (BoardHelpers.SquareIsInBounds((Row + _fwd, Col + 1)))
            targetedSquares.Add((Row + _fwd, Col + 1));

        return targetedSquares;
    }

    /// <summary>
    /// Returns the squares which the pawn can reach in a single standard move.
    /// This does not include En Passant squares.
    /// </summary>
    /// <param name="board"></param>
    /// <returns>A list of (row, col) tuples</returns>
    public override List<(int row, int col)> GetReachableSquares(Board board)
    {
        List<(int row, int col)> squares = [];

        // Add non-capturing move squares
        if (BoardHelpers.SquareIsInBounds((Row + _fwd, Col)) && board.State[Row + _fwd, Col] == null)
        {
            squares.Add((Row + _fwd, Col));

            if (HasMoved(board) == false && board.State[Row + 2 * _fwd, Col] == null)
            {
                squares.Add((Row + 2 * _fwd, Col));
            }
        }

        // Add standard capturing moves (not En Passant)
        if (BoardHelpers.SquareIsInBounds((Row + _fwd, Col - 1)) && 
            board.State[Row + _fwd, Col - 1]?.Color == ColorHelpers.OppositeColor(Color))
        {
            squares.Add((Row + _fwd, Col - 1));
        }
            
        if (BoardHelpers.SquareIsInBounds((Row + _fwd, Col + 1)) && 
            board.State[Row + _fwd, Col + 1]?.Color == ColorHelpers.OppositeColor(Color))
        {
            squares.Add((Row + _fwd, Col + 1));
        }
        
        return squares;
    }


    /// <summary>
    /// Returns a List of Moves including PromotionMoves and EnPassantMoves where possible.
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    public override List<IMove> GetValidMoves(Board board)
    {
        List<IMove> validMoves = [];

        // Get valid moves for standard and promotion moves
        foreach (var toSquare in GetReachableSquares(board))
        {
            if (board.MoveLeavesPlayerInCheck(Square, toSquare) == false)
            {
                if (toSquare.row == Board.MinIndex || toSquare.row == Board.MaxIndex)
                {
                    validMoves.Add(
                        new PromotionMove(Square, toSquare)
                    );
                }
                else
                {
                    validMoves.Add(
                        new StandardMove(Square, toSquare)
                    );
                }
            }
        }

        // Add En Passant move if available
        var enPassantSquares = GetEnPassantSquares(board);

        if (enPassantSquares != null)
        {
            var to = enPassantSquares.Value.to;
            var captured = enPassantSquares.Value.captured;

            if (board.MoveLeavesPlayerInCheck(Square, to, captured) == false)
            {
                validMoves.Add(
                    new EnPassantMove(Square, to, captured)
                );
            }
        }


        return validMoves;        
    }


    /// <summary>
    /// Returns a (row, col) tuple if En Passant is possible. Otherwise, returns null
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    public ((int row, int col) to, (int row, int col) captured)? GetEnPassantSquares(Board board)
    {
        // 1) Checks that there is a last move and that it is a standard move
        // 2) Checks that the last moving piece was a pawn
        // 3) Checks that the enemy pawn advanced two squares
        // 4) Checks that the enemy pawn is on the same row as this piece
        // 5) Checks that the enemy pawn is on an adjacent column to this piece

        if (board.MoveHistory.LastOrDefault() is Move lastMove &&       
            board.State[lastMove.To.row, lastMove.To.col]?.PieceType == PieceType.Pawn &&
            Math.Abs(lastMove.From.row - lastMove.To.row) == 2 &&
            lastMove.To.row == Row &&
            (Col == lastMove.To.col - 1 || Col == lastMove.To.col + 1)
        )
        {
            (int row, int col) to = (Row + _fwd, lastMove.To.col);
            (int row, int col) captured = lastMove.To;
            return (to, captured);
        }

        return null;
    }

}