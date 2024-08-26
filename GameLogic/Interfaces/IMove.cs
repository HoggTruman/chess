using GameLogic.Enums;

namespace GameLogic.Interfaces;

public interface IMove
{
    MoveType MoveType { get; }

    (int row, int col) From { get; }
    (int row, int col) To { get; }

    IPiece MovingPiece { get; }
}