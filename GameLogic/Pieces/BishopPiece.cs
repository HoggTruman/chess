using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Interfaces;
using GameLogic.Helpers;

namespace GameLogic.Pieces;

public class BishopPiece : IPiece
{
    public int Row { get; set; }
    public int Col { get; set; }

    public Color Color { get; }
    public PieceType PieceType { get; } = PieceType.Bishop;
    public int Value { get; }


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="row">Row index from 0 to 7</param>
    /// <param name="col">Col index from 0 to 7</param>
    /// <param name="color"></param>
    /// <param name="value">The value of the piece. Defaults to bishop value but can be set manually for pawn promotion</param>
    public BishopPiece(int row, int col, Color color=Color.White, int value=PieceValues.Bishop)
    {
        Row = row;
        Col = col;
        Color = color;
        Value = value;
    }

    public List<(int row, int col)> GetTargetedSquares(Board board)
    {
        return PieceHelpers.ScanDiagonals(Row, Col, board);
    }

    public List<(int row, int col)> GetValidMoves(Board board)
    {
        throw new NotImplementedException();
    }


}