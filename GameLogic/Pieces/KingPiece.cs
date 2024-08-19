using GameLogic.Constants;
using GameLogic.Enums;

namespace GameLogic.Pieces;

public class KingPiece : Piece
{
    public bool HasMoved { get; set; } = false;


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="row">Row index from 0 to 7</param>
    /// <param name="col">Col index from 0 to 7</param>
    /// <param name="color"></param>
    public KingPiece(int row, int col, Color color=Color.White)
        : base(row, col, color, PieceType.Bishop, PieceValues.King)
    {

    }


    public override List<(int row, int col)> GetTargetedSquares(Board board)
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

    public override List<(int row, int col)> GetReachableSquares(Board board)
    {
        throw new NotImplementedException();
    }

}