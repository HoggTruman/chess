using BetterGameLogic.Enums;

namespace BetterGameLogic.Moves;

/// <summary>
/// An En Passant move. Contains the captured Pawn's square
/// </summary>
public class EnPassantMove : SinglePieceMove
{
    public override MoveType MoveType => MoveType.EnPassant;
    public Square Captured { get; }

    public EnPassantMove(Square from, Square to, Square captured) 
        :base(from, to)
    {
        Captured = captured;
    }

    public override void Apply(Board board)
    {
        throw new NotImplementedException();
    }

    public override void Undo(Board board)
    {
        throw new NotImplementedException();
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