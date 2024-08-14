using GameLogic.Enums;

namespace GameLogic.Interfaces;

public interface IPiece
{
    int Row { get; set; }
    int Col { get; set; }

    Color Color { get; }
    int Value { get; }
    bool IsKing { get; }

    List<(int row, int col)> GetAvailableMoves(Board board);
    void Move(int row, int col);
    void Take(IPiece target);
}