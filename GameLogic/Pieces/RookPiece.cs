using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Interfaces;

namespace GameLogic.Pieces;

public class RookPiece : IPiece
{
    public int Row { get; set; }
    public int Col { get; set; }

    public Color Color { get; }
    public int Value { get; }

    public bool IsKing { get; } = false;
    public bool HasMoved { get; set; } = false;


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="row">Row index from 0 to 7</param>
    /// <param name="col">Col index from 0 to 7</param>
    /// <param name="color"></param>
    /// <param name="value">The value of the piece. Defaults to rook value but can be set manually for pawn promotion</param>
    public RookPiece(int row, int col, Color color=Color.White, int value=PieceValues.Rook)
    {
        Row = row;
        Col = col;
        Color = color;
        Value = value;
    }


    public List<(int row, int col)> GetTargetedSquares(Board board)
    {
        return PieceHelpers.ScanRowAndCol(Row, Col, board);
    }

    public List<(int row, int col)> GetValidMoves(Board board)
    {
        throw new NotImplementedException();
    }
}