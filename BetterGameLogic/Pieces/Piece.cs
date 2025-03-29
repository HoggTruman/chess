using BetterGameLogic.Enums;
using BetterGameLogic.Moves;

namespace BetterGameLogic.Pieces;

public abstract class Piece : IPiece
{
    protected readonly Board _board;

    public int Row { get; set; }
    public int Col { get; set; }
    public Square Square
    {
        get => new() { Row = Row, Col = Col };
        set
        {
            Row = value.Row;
            Col = value.Col;
        }
    }
    public Square StartSquare { get; }
    public PieceColor Color { get; }
    public abstract PieceType PieceType { get; }
    public abstract int Value { get; }


    protected Piece(Board board, int row, int col, PieceColor color, Square? startSquare = null)
    {
        _board = board;
        Row = row;
        Col = col;
        StartSquare = startSquare == null? new(row, col): startSquare.Value; // useful for promotion
        Color = color;
    } 

    public abstract List<Square> GetTargetedSquares();

    public abstract List<Square> GetReachableSquares();

    public virtual List<IMove> GetValidMoves()
    {
        List<IMove> validMoves = [];

        // keep the squares which represent a valid move
        foreach (var toSquare in GetReachableSquares())
        {
            StandardMove move = new(Square, toSquare);
            if (!move.LeavesPlayerInCheck(_board))
            {
                validMoves.Add(move);
            }
        }

        return validMoves;
    }


    public bool HasMoved()
    {
        return _board.History.HasMoved(this);
    }
}