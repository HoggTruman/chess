using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Interfaces;

namespace GameLogic.Pieces;

public class PawnPiece : Piece
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="row">Row index from 0 to 7</param>
    /// <param name="col">Col index from 0 to 7</param>
    /// <param name="color"></param>
    public PawnPiece(int row, int col, Color color=Color.White)
        : base(row, col, color, PieceType.Pawn, PieceValues.Pawn)
    {

    }


    /// <summary>
    /// Returns the targeted squares diagonally in front of a pawn based on color.
    /// En Passant is not considered here
    /// </summary>
    /// <param name="board"></param>
    /// <returns>A list of (row, col) tuples</returns>
    public override List<(int row, int col)> GetTargetedSquares(Board board)
    {
        List<(int row, int col)> targetedSquares = [];

        // Add targeted squares based on color of piece
        if (Color == Color.White)
        {
            targetedSquares = [
                new(Row + 1, Col - 1),
                new(Row + 1, Col + 1)
            ];
        }
        else if (Color == Color.Black)
        {
            targetedSquares = [
                new(Row - 1, Col - 1),
                new(Row - 1, Col + 1)
            ];
        }

        // Filter to keep only in bounds squares
        targetedSquares = BoardHelpers.KeepInBoundsSquares(targetedSquares);

        return targetedSquares;
    }


    public override List<(int row, int col)> GetReachableSquares(Board board)
    {
        List<(int row, int col)> squares = [];

        // Add non-capturing move squares
        if (Color == Color.White)
        {
            if (Row + 1 <= Board.MaxIndex && board.State[Row + 1, Col] == null)
            {
                squares.Add(new(Row + 1, Col));

                if (HasMoved(board) == false && board.State[Row + 2, Col] == null)
                {
                    squares.Add(new(Row + 2, Col));
                }
            }
        }
        else if (Color == Color.Black)
        {
            if (Row - 1 >= Board.MinIndex && board.State[Row - 1, Col] == null)
            {
                squares.Add(new(Row - 1, Col));

                if (HasMoved(board) == false && board.State[Row - 2, Col] == null)
                {
                    squares.Add(new(Row - 2, Col));
                }
            }
        }

        // Add standard capturing moves (not En Passant)
        if (Color == Color.White)
        {
            if (Col - 1 >= Board.MinIndex && Row + 1 <= Board.MaxIndex && board.State[Row + 1, Col - 1]?.Color == Color.Black)
                squares.Add(new(Row + 1, Col - 1));
            
            if (Col + 1 <= Board.MaxIndex && Row + 1 <= Board.MaxIndex && board.State[Row + 1, Col + 1]?.Color == Color.Black)
                squares.Add(new(Row + 1, Col + 1));
        }
        else if (Color == Color.Black)
        {
            if (Col - 1 >= Board.MinIndex && Row - 1 >= Board.MinIndex && board.State[Row - 1, Col - 1]?.Color == Color.Black)
                squares.Add(new(Row - 1, Col - 1));
            
            if (Col + 1 <= Board.MaxIndex && Row - 1 >= Board.MinIndex && board.State[Row - 1, Col + 1]?.Color == Color.Black)
                squares.Add(new(Row - 1, Col + 1));
        }

        // Add En Passant square if there is one
        var enPassantSquare = GetEnPassantSquare(board);
        if (enPassantSquare != null)
            squares.Add(((int row, int col))enPassantSquare);

        
        return squares;
    }

    private (int row, int col)? GetEnPassantSquare(Board board)
    {
        IMove? lastMove = board.MoveHistory.LastOrDefault();

        if (
            lastMove != null &&
            lastMove.MovingPiece.PieceType == PieceType.Pawn &&
            Math.Abs(lastMove.From.row - lastMove.To.row) == 2 &&
            lastMove.To.row == Row 
        )
        {
            if (Col == lastMove.To.col - 1 || Col == lastMove.To.col + 1)
            {
                return ((lastMove.From.row + lastMove.To.row) / 2, lastMove.To.col);
            }
        }

        return null;
    }



}