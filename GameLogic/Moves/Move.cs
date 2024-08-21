using GameLogic.Enums;
using GameLogic.Interfaces;

namespace GameLogic.Moves;

public class Move : IMove
{
    public MoveType MoveType { get; } = MoveType.Move;
    public (int row, int col) From { get; }
    public (int row, int col) To { get; }
    public IPiece MovingPiece { get; }


    public Move((int row, int col) from, (int row, int col) to, IPiece movingPiece)
    {
        From = from;
        To = to;
        MovingPiece = movingPiece;
    }
}