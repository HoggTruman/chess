using GameLogic.Enums;

namespace GameLogic.Moves;

/// <summary>
/// An En Passant move. Contains the captured Pawn's square
/// </summary>
public class EnPassantMove : SinglePieceMove
{
    public (int row, int col) Captured { get; }

    public EnPassantMove(
        (int row, int col) from, 
        (int row, int col) to,
        (int row, int col) captured
    ) 
        :base(MoveType.EnPassant, from, to)
    {
        Captured = captured;
    }
}