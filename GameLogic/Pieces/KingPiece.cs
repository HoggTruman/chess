using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Interfaces;

namespace GameLogic.Pieces;

public class KingPiece : IPiece
{
    public int Row { get; set; }
    public int Col { get; set; }

    public Color Color { get; }
    public PieceType PieceType { get; } = PieceType.King;
    public int Value { get; } = PieceValues.King;

    public bool HasMoved { get; set; } = false;


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="row">Row index from 0 to 7</param>
    /// <param name="col">Col index from 0 to 7</param>
    /// <param name="color"></param>
    public KingPiece(int row, int col, Color color=Color.White)
    {
        Row = row;
        Col = col;
        Color = color;
    }


    public List<(int row, int col)> GetTargetedSquares(Board board)
    {
        // Get all squares in range (including those out of bounds)
        List<(int row, int col)> targetedSquares = [
            new(Row - 1, Col),
            new(Row + 1, Col),
            new(Row, Col - 1),
            new(Row, Col + 1),
            new(Row - 1, Col - 1),
            new(Row - 1, Col + 1),
            new(Row + 1, Col - 1),
            new(Row + 1, Col + 1)
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