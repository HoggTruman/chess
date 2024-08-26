using GameLogic.Enums;
using GameLogic.Interfaces;

namespace GameLogic.Moves;

public class Move : IMove
{
    public MoveType MoveType { get; }

    public (int row, int col) From { get; }
    public (int row, int col) To { get; }

    public IPiece MovingPiece { get; }


    public Move(
        MoveType moveType,
        (int row, int col) from, 
        (int row, int col) to,
        IPiece movingPiece
    )
    {
        MoveType = moveType;
        From = from;
        To = to;
        MovingPiece = movingPiece;
    }
}