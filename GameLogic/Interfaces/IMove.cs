using GameLogic.Enums;

namespace GameLogic.Interfaces;

public interface IMove
{
    MoveType MoveType { get; }

    (int row, int col) To { get; }

    bool MovesSquare((int row, int col) square);
}