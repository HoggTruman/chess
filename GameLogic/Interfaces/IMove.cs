using GameLogic.Enums;

namespace GameLogic.Interfaces;

public interface IMove
{
    MoveType MoveType { get; }

    bool MovesSquare((int row, int col) square);
}