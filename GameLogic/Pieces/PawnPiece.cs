using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Interfaces;
using GameLogic.Moves;

namespace GameLogic.Pieces;

public class PawnPiece : Piece
{
    #region Fields

    /// <summary>
    /// The forward direction for the piece (+1 for White, -1 for Black)
    /// </summary>
    private readonly int _fwd;

    #endregion



    #region Constructors

    /// <summary>
    /// Initializes a new instance of the PawnPiece class.
    /// </summary>
    /// <param name="board">The Board object the piece will be placed on.</param>
    /// <param name="row">Row index from 0 to 7.</param>
    /// <param name="col">Column index from 0 to 7.</param>
    /// <param name="color">The Color of the piece.</param>
    public PawnPiece(Board board, int row, int col, Color color)
        : base(board, row, col, color, PieceType.Pawn, PieceValues.Pawn)
    {
        _fwd = color == Color.White? 1: -1;
    }

    #endregion



    #region Public Methods

    /// <summary>
    /// Returns the targeted squares diagonally in front of a pawn.
    /// En Passant is not considered here.
    /// </summary>
    /// <returns>A list of (row, col) tuples</returns>
    public override List<(int row, int col)> GetTargetedSquares()
    {
        List<(int row, int col)> targetedSquares = [];

        // Add targeted squares (if in bounds)
        if (BoardHelpers.SquareIsInBounds((Row + _fwd, Col - 1)))
            targetedSquares.Add((Row + _fwd, Col - 1));
        
        if (BoardHelpers.SquareIsInBounds((Row + _fwd, Col + 1)))
            targetedSquares.Add((Row + _fwd, Col + 1));

        return targetedSquares;
    }


    /// <summary>
    /// Returns the squares which the pawn can reach in a single standard move.
    /// This does not include En Passant squares.
    /// </summary>
    /// <returns>A list of (row, col) tuples</returns>
    public override List<(int row, int col)> GetReachableSquares()
    {
        List<(int row, int col)> squares = [];

        // Add non-capturing move squares
        if (BoardHelpers.SquareIsInBounds((Row + _fwd, Col)) 
            && _board.State[Row + _fwd, Col] == null)
        {
            squares.Add((Row + _fwd, Col));

            if (HasMoved() == false && _board.State[Row + 2 * _fwd, Col] == null)
            {
                squares.Add((Row + 2 * _fwd, Col));
            }
        }

        // Add standard capturing moves (not En Passant)
        if (BoardHelpers.SquareIsInBounds((Row + _fwd, Col - 1)) && 
            _board.State[Row + _fwd, Col - 1]?.Color == ColorHelpers.OppositeColor(Color))
        {
            squares.Add((Row + _fwd, Col - 1));
        }
            
        if (BoardHelpers.SquareIsInBounds((Row + _fwd, Col + 1)) && 
            _board.State[Row + _fwd, Col + 1]?.Color == ColorHelpers.OppositeColor(Color))
        {
            squares.Add((Row + _fwd, Col + 1));
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
        foreach (var toSquare in GetReachableSquares())
        {
            if (_board.MoveLeavesPlayerInCheck(Square, toSquare) == false)
            {
                if (toSquare.row == Board.MinIndex || toSquare.row == Board.MaxIndex)
                {
                    validMoves.Add(
                        new PromotionMove(Square, toSquare)
                    );
                }
                else
                {
                    validMoves.Add(
                        new StandardMove(Square, toSquare)
                    );
                }
            }
        }

        // Add En Passant move if available
        var enPassantSquares = GetEnPassantSquares();

        if (enPassantSquares != null)
        {
            var to = enPassantSquares.Value.to;
            var captured = enPassantSquares.Value.captured;

            if (_board.MoveLeavesPlayerInCheck(Square, to, captured) == false)
            {
                validMoves.Add(
                    new EnPassantMove(Square, to, captured)
                );
            }
        }

        return validMoves;        
    }


    /// <summary>
    /// Returns a (row, col) tuple if En Passant is possible. Otherwise, returns null
    /// </summary>
    /// <returns></returns>
    public ((int row, int col) to, (int row, int col) captured)? GetEnPassantSquares()
    {
        // 1) Checks that there is a last move and that it is a standard move
        // 2) Checks that the last moving piece was a pawn
        // 3) Checks that the enemy pawn advanced two squares
        // 4) Checks that the enemy pawn is on the same row as this piece
        // 5) Checks that the enemy pawn is on an adjacent column to this piece

        if (_board.MoveHistory.LastOrDefault() is StandardMove lastMove &&       
            _board.State[lastMove.To.row, lastMove.To.col]?.PieceType == PieceType.Pawn &&
            Math.Abs(lastMove.From.row - lastMove.To.row) == 2 &&
            lastMove.To.row == Row &&
            (Col == lastMove.To.col - 1 || Col == lastMove.To.col + 1)
        )
        {
            (int row, int col) to = (Row + _fwd, lastMove.To.col);
            (int row, int col) captured = lastMove.To;
            return (to, captured);
        }

        return null;
    }
    #endregion
}