using BetterGameLogic.Enums;
using BetterGameLogic.Pieces;

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
        if (board.At(From) == null)
        {
            throw new InvalidOperationException("The From square is empty");
        }

        board.RemoveAt(Captured);
        board.MovePiece(From, To);
    }

    public override void Undo(Board board, IPiece? capturedPiece)
    {
        if (capturedPiece == null)
        {
            throw new ArgumentException("capturedPiece can not be null for an EnPassantMove");
        }

        if (board.At(To) == null)
        {
            throw new InvalidOperationException("The To square is empty");
        }

        board.MovePiece(To, From);
        board.AddPiece(capturedPiece);        
    }

    public override bool LeavesPlayerInCheck(Board board)
    {
        IPiece? movingPiece = board.At(From);

        if (movingPiece == null)
        {
            throw new InvalidOperationException("There is no piece on the From square.");
        }
        
        IPiece? captured = board.At(Captured);
        Apply(board);
        bool result = board.GetKing(movingPiece.Color).IsUnderCheck();
        Undo(board, captured);
        return result;
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