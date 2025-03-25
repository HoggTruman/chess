namespace BetterGameLogic;

public readonly struct Square
{
    public Square(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public int Row { get; init; }
    public int Col { get; init; }
}
