using GameLogic.Constants;
using GameLogic.Enums;

namespace GameLogic.Pieces;

public class PawnPiece : Piece
{
    public bool HasMoved { get; set; } = false;

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
        targetedSquares = targetedSquares.Where(p => 
            p.row >= Board.MinIndex &&
            p.row <= Board.MaxIndex &&
            p.col >= Board.MinIndex &&
            p.col <= Board.MaxIndex
        ).ToList();

        return targetedSquares;
    }

    public override List<(int row, int col)> GetReachableSquares(Board board)
    {
        throw new NotImplementedException();
    }

}