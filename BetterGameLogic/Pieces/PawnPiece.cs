using BetterGameLogic.Constants;
using BetterGameLogic.Enums;
using BetterGameLogic.Helpers;
using BetterGameLogic.Moves;

namespace BetterGameLogic.Pieces;

public class PawnPiece : Piece
{
    /// <summary>
    /// The forward direction for the piece (+1 for White, -1 for Black)
    /// </summary>
    private readonly int _fwd;

    public override PieceType PieceType => PieceType.Pawn;
    public override int Value => PieceValues.Pawn;


    public PawnPiece(Board board, int row, int col, PieceColor color, Square? startSquare = null)
        : base(board, row, col, color, startSquare)
    {
        _fwd = color == PieceColor.White? -1: 1;
    }

    public PawnPiece(Board board, Square square, PieceColor color, Square? startSquare = null) 
        : this(board, square.Row, square.Col, color, startSquare)
    {
        
    }



    /// <summary>
    /// Returns the targeted squares diagonally in front of a pawn.
    /// En Passant is not considered here.
    /// </summary>
    /// <returns>A list of (row, col) tuples</returns>
    public override List<Square> GetTargetedSquares()
    {
        List<Square> targetedSquares = [];

        if (Board.IsInBounds(Row + _fwd, Col - 1))
        {
            targetedSquares.Add(new(Row + _fwd, Col - 1));
        }
            
        if (Board.IsInBounds(Row + _fwd, Col + 1))
        {
            targetedSquares.Add(new(Row + _fwd, Col + 1));
        }            

        return targetedSquares;
    }


    /// <summary>
    /// Returns the squares which the pawn can reach in a single standard move.
    /// This does not include En Passant squares.
    /// </summary>
    /// <returns>A list of (row, col) tuples</returns>
    public override List<Square> GetReachableSquares()
    {
        List<Square> squares = [];

        // Add non-capturing move squares
        if (Board.IsInBounds(Row + _fwd, Col) && 
            _board.State[Row + _fwd, Col] == null)
        {
            squares.Add(new(Row + _fwd, Col));

            if (HasMoved() == false && _board.State[Row + 2 * _fwd, Col] == null)
            {
                squares.Add(new(Row + 2 * _fwd, Col));
            }
        }

        // Add standard capturing moves (not En Passant)
        if (Board.IsInBounds(Row + _fwd, Col - 1) && 
            _board.IsOccupiedByColor(Row + _fwd, Col - 1, ColorHelpers.Opposite(Color)))
        {
            squares.Add(new(Row + _fwd, Col - 1));
        }
            
        if (Board.IsInBounds(Row + _fwd, Col + 1) && 
            _board.IsOccupiedByColor(Row + _fwd, Col + 1, ColorHelpers.Opposite(Color)))
        {
            squares.Add(new(Row + _fwd, Col + 1));
        }
        
        return squares;
    }


    /// <summary>
    /// Returns a List of Moves including PromotionMoves and EnPassantMoves where possible.
    /// </summary>
    /// <returns></returns>
    public override List<IMove> GetValidMoves()
    {
        List<IMove> validMoves = [];

        // Get valid moves for standard and promotion moves
        foreach (Square to in GetReachableSquares())
        {
            // Only the standard move needs to be tested for leaving in check (promotion moves give the same result)
            StandardMove standardMove = new(Square, to);
            if (!standardMove.LeavesPlayerInCheck(_board))
            {
                if (to.Row == 0 || to.Row == Board.BoardSize - 1)
                {
                    validMoves.Add(new PromotionMove(Square, to, PieceType.Queen));
                    validMoves.Add(new PromotionMove(Square, to, PieceType.Rook));
                    validMoves.Add(new PromotionMove(Square, to, PieceType.Knight));
                    validMoves.Add(new PromotionMove(Square, to, PieceType.Bishop));
                }
                else
                {
                    validMoves.Add(standardMove);
                }
            }
        }

        // Add En Passant move if available
        var enPassantSquare = GetEnPassantSquare();

        if (enPassantSquare != null)
        {
            EnPassantMove enPassantMove = new(Square, enPassantSquare.Value);

            if (!enPassantMove.LeavesPlayerInCheck(_board))
            {
                validMoves.Add(enPassantMove);
            }
        }

        return validMoves;        
    }


    /// <summary>
    /// Returns the "To" square if En Passant is possible. Otherwise, returns null
    /// </summary>
    /// <returns></returns>
    public Square? GetEnPassantSquare()
    {
        // 1) Checks that there is a last move and that it is a standard move
        // 2) Checks that the last moving piece was a pawn
        // 3) Checks that the enemy pawn advanced two squares
        // 4) Checks that the enemy pawn is on the same row as this piece
        // 5) Checks that the enemy pawn is on an adjacent column to this piece

        if (_board.History.LastOrDefault()?.Move is StandardMove lastMove &&       
            _board.At(lastMove.To)?.PieceType == PieceType.Pawn &&
            Math.Abs(lastMove.From.Row - lastMove.To.Row) == 2 &&
            lastMove.To.Row == Row &&
            (Col == lastMove.To.Col - 1 || Col == lastMove.To.Col + 1))
        {
            Square to = new(Row + _fwd, lastMove.To.Col);
            return to;
        }

        return null;
    }
}