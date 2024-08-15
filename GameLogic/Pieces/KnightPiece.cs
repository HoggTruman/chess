using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Interfaces;

namespace GameLogic.Pieces;

public class KnightPiece : IPiece
{
    public int Row { get; set; }
    public int Col { get; set; }

    public Color Color { get; }
    public int Value { get; }

    public bool IsKing { get; } = false;


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="row">Row index from 0 to 7</param>
    /// <param name="col">Col index from 0 to 7</param>
    /// <param name="color"></param>
    /// <param name="value">The value of the piece. Defaults to knight value but can be set manually for pawn promotion</param>
    public KnightPiece(int row, int col, Color color=Color.White, int value=PieceValues.Knight)
    {
        Row = row;
        Col = col;
        Color = color;
        Value = value;
    }


    public List<(int row, int col)> GetTargetedSquares(Board board)
    {
        // Get all squares in range (including those out of bounds)
        List<(int row, int col)> targetedSquares = [
            new(Row - 2, Col - 1),
            new(Row - 2, Col + 1),
            new(Row + 2, Col - 1),
            new(Row + 2, Col + 1),
            new(Row - 1, Col - 2),
            new(Row + 1, Col - 2),
            new(Row - 1, Col + 2),
            new(Row + 1, Col + 2)
        ];

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