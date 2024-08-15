using GameLogic.Interfaces;

namespace GameLogic;

public class Board
{
    public const int BoardSize = 8;
    public const int MinIndex = 0;
    public const int MaxIndex = BoardSize - 1;

    public IPiece?[,] State { get; set; }

    public Board()
    {
        State = new IPiece?[BoardSize, BoardSize];
    }
}