using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Interfaces;
using GameLogic.Moves;
using GameLogic.Pieces;



namespace GameLogic;

public class Board
{
    #region Constants

    public const int BoardSize = 8;
    public const int MinIndex = 0;
    public const int MaxIndex = BoardSize - 1;

    #endregion



    #region Properties

    public IPiece?[,] State { get; set; }

    public Dictionary<Color, KingPiece?> Kings { get; }

    public Dictionary<Color, List<IPiece>> Pieces { get; }

    public List<IMove> MoveHistory { get; } = [];

    #endregion



    #region Constructors

    /// <summary>
    /// Initializes a new instance of the Board class.
    /// </summary>
    public Board()
    {
        State = new IPiece?[BoardSize, BoardSize];
        Kings = new()
        {
            [Color.White] = null,
            [Color.Black] = null
        };
        Pieces = new()
        {
            [Color.White] = [],
            [Color.Black] = [],
        };
    }

    #endregion



    #region Public Methods

    /// <summary>
    /// Adds a new piece to the Board of specified PieceType at (row, col).
    /// New pieces should always be created in this way.
    /// </summary>
    /// <typeparam name="T">A child of the Piece abstract class</typeparam>
    /// <param name="row">The row index to place the new piece at.</param>
    /// <param name="col">The column index to place the new piece at.</param>
    /// <param name="color">The Color of the new piece.</param>
    /// <returns>The new piece of type <typeparamref name="T"/></returns>
    /// <exception cref="ArgumentException"></exception>
    public T AddNewPiece<T>(int row, int col, Color color=Color.White) where T : Piece
    {
        // Ensure the square is empty before creating a piece there
        if (State[row, col] != null)
        {
            throw new ArgumentException($"Board already contains a piece at (row: {row}, column: {col})");
        }

        // Create a piece of type T
        T piece = (T)(Activator.CreateInstance(typeof(T), this, row, col, color) ?? throw new Exception());

        // Handle king case
        if (typeof(T) == typeof(KingPiece))
        {
            if (Kings[color] != null)
            {   
                throw new ArgumentException($"{color} King already exists on the board");
            }

            Kings[color] = piece as KingPiece;
        }
        
        // Update the board state with the piece
        State[row, col] = piece;

        // Update corresponding piece list
        Pieces[color].Add(piece);
        
            
        return piece;
    }


    /// <summary>
    /// Adds a new piece to the Board of specified PieceType at the provided square.
    /// New pieces should always be created in this way.
    /// </summary>
    /// <typeparam name="T">A child of the Piece abstract class</typeparam>
    /// <param name="square">The (row, column) square to place the new piece at.</param>
    /// <param name="color">The Color of the new piece.</param>
    /// <returns>The new piece of type <typeparamref name="T"/></returns>
    public T AddNewPiece<T>((int row, int col) square, Color color = Color.White) where T : Piece
    {
        return AddNewPiece<T>(square.row, square.col, color);
    }


    // REFACTOR OUT LATER
    public void HandleMove(IMove move)
    {
        MoveHistory.Add(move);

        if (move.MoveType == MoveType.Standard)
        {
            StandardMove((StandardMove)move);
        }
        else if (move.MoveType == MoveType.EnPassant)
        {
            EnPassant((EnPassantMove)move);
        }
        else if (move.MoveType == MoveType.Promotion)
        {
            PawnPromote((PromotionMove)move);
        }
        // add castle handling
    }


    /// <summary>
    /// Determines if a player will leave themself in check by moving the piece at "from" to "to".
    /// For En Passant, pass in the captured pawn's square as the "captured" parameter.
    /// Castling does its own verification and is not passed through this method.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="captured"></param>
    /// <returns>true if the player would be in check after the move. Otherwise, false</returns>
    /// <exception cref="ArgumentException"></exception>
    public bool MoveLeavesPlayerInCheck((int row, int col) from, (int row, int col) to, (int row, int col)? captured = null)
    {
        // Ensure there the moving piece is not null
        IPiece? movingPiece = State[from.row, from.col];

        if (movingPiece == null)
        {
            throw new ArgumentException($"Piece not found at \"from\": (row: {from.row}, col: {from.col})");
        }
            
        // Return false when no king.
        KingPiece? playerKing = Kings[movingPiece.Color];
        if (playerKing == null)
        {
            return false;
        }

        // Get target piece
        IPiece? capturedPiece = State[to.row, to.col];

        if (captured != null)
        {
            // Set capturedPiece for En Passant
            capturedPiece = State[captured.Value.row, captured.Value.col];

            // Set the "captured" square to null for En Passant
            State[captured.Value.row, captured.Value.col] = null;
        }

        // Make the move
        State[from.row, from.col] = null;
        State[to.row, to.col] = movingPiece;
        movingPiece.Square = to;

        if (capturedPiece != null)
        {
            Pieces[capturedPiece.Color].Remove(capturedPiece);
        }

        // Record the result
        bool result = playerKing.IsChecked();

        // Roll back pieces
        State[from.row, from.col] = movingPiece;
        movingPiece.Square = from;
        
        if (captured != null)
        {
            // En Passant revert
            State[to.row, to.col] = null;
            State[captured.Value.row, captured.Value.col] = capturedPiece;
        }
        else
        {
            State[to.row, to.col] = capturedPiece;
        }

        if (capturedPiece != null)
        {
            Pieces[capturedPiece.Color].Add(capturedPiece);
        }

        
        return result;
    }

    #endregion



    #region Private Methods

    /// <summary>
    /// Updates the State of the board to reflect the StandardMove. 
    /// Updates the Row and Col properties of the moving piece.
    /// Removes captured piece from Pieces
    /// </summary>
    /// <param name="move"></param>
    private void StandardMove(StandardMove move)
    {
        // Remove captured piece from Pieces
        var capturedPiece = State[move.To.row, move.To.col];
        if (capturedPiece != null)
        {
            Pieces[capturedPiece.Color].Remove(capturedPiece);
            State[move.To.row, move.To.col] = null;
        }

        // Move the piece 
        MovePiece(move.From, move.To);
    }


    /// <summary>
    /// Updates the State of the board to reflect the En Passant move. 
    /// Updates the Row and Col properties of the moving pawn.
    /// Removes captured pawn from Pieces
    /// </summary>
    /// <param name="move"></param>
    private void EnPassant(EnPassantMove move)
    {
        // Remove captured pawn from the board
        var capturedPawn = State[move.Captured.row, move.Captured.col];
        if (capturedPawn != null)
        {
            Pieces[capturedPawn.Color].Remove(capturedPawn);
            State[move.Captured.row, move.Captured.col] = null;
        }

        // Move the pawn
        MovePiece(move.From, move.To);
    }


    /// <summary>
    /// Updates <see cref="State"/> with a new queen, rook, knight or bishop replacing the pawn. <br/>
    /// Replaces the pawn in <see cref="Pieces"/> with the new piece. <br/>
    /// If the move captures a piece, it is removed from Pieces.
    /// </summary>
    /// <param name="move">A PromotionMove instance</param>
    /// <exception cref="ArgumentException"></exception>
    private void PawnPromote(PromotionMove move)
    {
        // Remove pawn
        var pawn = State[move.From.row, move.From.col];
        if (pawn == null)
        {
            throw new ArgumentException($"Piece not found at: (row: {move.From.row}, col:{move.From.col})");
        }
        Pieces[pawn.Color].Remove(pawn);
        State[move.From.row, move.From.col] = null;

        // Remove captured piece
        var capturedPiece = State[move.To.row, move.To.col];
        if (capturedPiece != null)
        {
            Pieces[capturedPiece.Color].Remove(capturedPiece);
            State[move.To.row, move.To.col] = null;
        }

        // Create promoted piece
        if (move.PromotedTo == PieceType.Queen)
        {
            AddNewPiece<QueenPiece>(move.To, pawn.Color);
        }
        else if (move.PromotedTo == PieceType.Rook)
        {
            AddNewPiece<RookPiece>(move.To, pawn.Color);
        }
        else if (move.PromotedTo == PieceType.Knight)
        {
            AddNewPiece<KnightPiece>(move.To, pawn.Color);
        }
        else if (move.PromotedTo == PieceType.Bishop)
        {
            AddNewPiece<BishopPiece>(move.To, pawn.Color);
        }
        else
        {
            throw new ArgumentException(@$"{move.PromotedTo} is not a valid promotion option." + 
                " A pawn can only be promoted to a queen, rook, knight or bishop.");
        }
    }


    // king location may not be needed since king always castles from same square
    private void Castle((int row, int col) kingFrom, (int row, int col) rookFrom)
    {
        throw new NotImplementedException();
    }



    /// <summary>
    /// Updates the State of the board to reflect the move. 
    /// Updates the Row and Col properties of the moving piece.
    /// </summary>
    /// <param name="from">The square the piece is moving from</param>
    /// <param name="to">The square the piece is moving to</param>
    /// <exception cref="ArgumentException"></exception>
    private void MovePiece((int row, int col) from, (int row, int col) to)
    {
        var movingPiece = State[from.row, from.col];

        if (movingPiece == null)
        {
            throw new ArgumentException($"A piece does not exist on the board at (row:{from.row}, col: {from.col})");
        }

        // Update State for the movingPiece
        State[to.row, to.col] = movingPiece;
        State[from.row, from.col] = null;
        
        // Update the movingPiece row and col
        movingPiece.Square = to;
    }

    #endregion
}