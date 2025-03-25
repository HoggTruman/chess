namespace BetterGameLogic;

public readonly record struct Square
{
    public Square(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public int Row { get; init; }
    public int Col { get; init; }
}
