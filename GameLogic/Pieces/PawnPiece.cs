using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Interfaces;

namespace GameLogic.Pieces;

public class PawnPiece : IPiece
{
    public int Row { get; set; }
    public int Col { get; set; }

    public Color Color { get; }
    public int Value { get; } = PieceValues.Pawn;

    public bool IsKing { get; } = false;
    public bool HasMoved { get; set; } = false;


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="row">Row index from 0 to 7</param>
    /// <param name="col">Col index from 0 to 7</param>
    /// <param name="color"></param>
    public PawnPiece(int row, int col, Color color=Color.White)
    {
        Row = row;
        Col = col;
        Color = color;
    }


    /// <summary>
    /// Returns the targeted squares diagonally in front of a pawn based on color.
    /// En Passant is not considered here
    /// </summary>
    /// <param name="board"></param>
    /// <returns>A list of (row, col) tuples</returns>
    public List<(int row, int col)> GetTargetedSquares(Board board)
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


    public List<(int row, int col)> GetValidMoves(Board board)
    {
        throw new NotImplementedException();
    }
}