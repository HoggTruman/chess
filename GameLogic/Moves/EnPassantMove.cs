using GameLogic.Enums;
using GameLogic.Interfaces;

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

    public override bool IsEquivalentTo(IMove move)
    {
        if (move.MoveType != MoveType)
        {
            return false;
        }

        EnPassantMove enPassantMove = (EnPassantMove)move;

        return enPassantMove.From == From &&
               enPassantMove.To == To &&
               enPassantMove.Captured == Captured;
    }
}