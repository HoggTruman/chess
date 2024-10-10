using GameLogic.Enums;

namespace GameLogic.Interfaces;

public interface IMove
{
    MoveType MoveType { get; }

    (int row, int col) From { get; }
    (int row, int col) To { get; }

    bool MovesSquare((int row, int col) square);

    /// <summary>
    /// Compares if all property values are equal to the provided IMove.
    /// </summary>
    /// <param name="move">The IMove to compare against.</param>
    /// <returns>true if equivalent. Otherwise, false.</returns>
    bool IsEquivalentTo(IMove move);
}