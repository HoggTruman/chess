using BetterGameLogic.Enums;
using BetterGameLogic.Pieces;

namespace BetterGameLogic.Moves;

/// <summary>
/// A Pawn Promotion move. Contains the PieceType promoted to 
/// </summary>
public record PromotionMove : Move
{
    public override MoveType MoveType => MoveType.Promotion;

    /// <summary>
    /// The type of piece the player chooses to promote to.
    /// Its value will be filled once the player makes the move.
    /// </summary>
    public PieceType PromotedTo { get; set; }


    public PromotionMove(Square from, Square to, PieceType promotedTo) 
        :base(from, to)
    {
        PromotedTo = promotedTo;
    }


    protected override void ApplyWithoutUpdatingHistory(Board board)
    {
        IPiece? pawn = board.At(From);
        if (pawn == null)
        {
            throw new InvalidOperationException("The From square is empty");
        }
        
        board.RemoveAt(To);

        IPiece promotedPiece;
        if (PromotedTo == PieceType.Queen) promotedPiece = new QueenPiece(board, To, pawn.Color, pawn.StartSquare);
        else if (PromotedTo == PieceType.Rook) promotedPiece = new RookPiece(board, To, pawn.Color, pawn.StartSquare);
        else if (PromotedTo == PieceType.Knight) promotedPiece = new KnightPiece(board, To, pawn.Color, pawn.StartSquare);
        else if (PromotedTo == PieceType.Bishop) promotedPiece = new BishopPiece(board, To, pawn.Color, pawn.StartSquare);
        else throw new ArgumentException(@$"{PromotedTo} is not a valid promotion option.");
        
        board.RemoveAt(From);
        board.AddPiece(promotedPiece);
    }

    protected override void UndoWithoutUpdatingHistory(Board board, IPiece? capturedPiece)
    {
        IPiece? promotedPiece = board.At(To);
        if (promotedPiece == null)
        {
            throw new InvalidOperationException("The To square is empty");
        }

        PawnPiece pawn = new(board, From, promotedPiece.Color, promotedPiece.StartSquare);
        board.AddPiece(pawn);
        board.RemoveAt(To);

        if (capturedPiece != null)
        {
            board.AddPiece(capturedPiece);
        }    
    }
}