using GameLogic.Interfaces;

namespace GameLogic;

public class Board
{
    public const int BoardSize = 8;
    public IPiece?[,] State { get; set; }

    public Board()
    {
        State = new IPiece?[BoardSize, BoardSize];
    }
}