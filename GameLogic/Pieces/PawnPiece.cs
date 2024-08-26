using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Interfaces;

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
    /// Returns the squares which the pawn can reach in a single move
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

        // Add En Passant square if there is one
        var enPassantSquare = GetEnPassantSquare(board);
        if (enPassantSquare != null)
            squares.Add(((int row, int col))enPassantSquare);

        
        return squares;
    }

    /// <summary>
    /// Returns a (row, col) tuple if En Passant is possible. Otherwise, returns null
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    public (int row, int col)? GetEnPassantSquare(Board board)
    {
        IMove? lastMove = board.MoveHistory.LastOrDefault();

        if (
            lastMove != null &&
            board.State[lastMove.To.row, lastMove.To.col]?.PieceType == PieceType.Pawn &&
            Math.Abs(lastMove.From.row - lastMove.To.row) == 2 &&
            lastMove.To.row == Row 
        )
        {
            if (Col == lastMove.To.col - 1 || Col == lastMove.To.col + 1)
            {
                return (lastMove.To.row + _fwd, lastMove.To.col);
            }
        }

        return null;
    }

}