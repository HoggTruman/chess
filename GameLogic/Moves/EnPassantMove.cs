using GameLogic.Enums;
using GameLogic.Interfaces;

namespace GameLogic.Moves;

public class EnPassantMove : Move
{
    public (int row, int col) Captured{ get; }

    public EnPassantMove(
        (int row, int col) from, 
        (int row, int col) to,
        IPiece movingPiece,
        (int row, int col) captured
    ) 
        :base(MoveType.EnPassant, from, to, movingPiece)
    {
        Captured = captured;
    }
}