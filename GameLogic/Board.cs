using GameLogic.Constants;
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

    public Dictionary<PieceColor, KingPiece> Kings { get; }

    public Dictionary<PieceColor, List<IPiece>> Pieces { get; }

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
            [PieceColor.White] = null,
            [PieceColor.Black] = null
        };
        Pieces = new()
        {
            [PieceColor.White] = [],
            [PieceColor.Black] = [],
        };
    }

    #endregion



    #region Public Methods

    /// <summary>
    /// Sets up the board with pieces in position for a new game.
    /// The board must be empty to use this method.
    /// </summary>
    /// <exception cref="Exception"></exception>
    public void Initialize()
    {
        // Ensure the board is empty
        if (Pieces[PieceColor.White].Count != 0 || Pieces[PieceColor.Black].Count != 0)
        {
            throw new Exception("Board must be empty to be Initialized.");
        }

        // Add White pieces
        AddNewPiece<RookPiece>(StartSquares.WhiteRookQ, PieceColor.White);
        AddNewPiece<KnightPiece>(StartSquares.WhiteKnightQ, PieceColor.White);
        AddNewPiece<BishopPiece>(StartSquares.WhiteBishopQ, PieceColor.White);
        AddNewPiece<QueenPiece>(StartSquares.WhiteQueen, PieceColor.White);
        AddNewPiece<KingPiece>(StartSquares.WhiteKing, PieceColor.White);
        AddNewPiece<BishopPiece>(StartSquares.WhiteBishopK, PieceColor.White);
        AddNewPiece<KnightPiece>(StartSquares.WhiteKnightK, PieceColor.White);
        AddNewPiece<RookPiece>(StartSquares.WhiteRookK, PieceColor.White);

        AddNewPiece<PawnPiece>(StartSquares.WhitePawnA, PieceColor.White);
        AddNewPiece<PawnPiece>(StartSquares.WhitePawnB, PieceColor.White);
        AddNewPiece<PawnPiece>(StartSquares.WhitePawnC, PieceColor.White);
        AddNewPiece<PawnPiece>(StartSquares.WhitePawnD, PieceColor.White);
        AddNewPiece<PawnPiece>(StartSquares.WhitePawnE, PieceColor.White);
        AddNewPiece<PawnPiece>(StartSquares.WhitePawnF, PieceColor.White);
        AddNewPiece<PawnPiece>(StartSquares.WhitePawnG, PieceColor.White);
        AddNewPiece<PawnPiece>(StartSquares.WhitePawnH, PieceColor.White);

        // Add Black pieces
        AddNewPiece<RookPiece>(StartSquares.BlackRookQ, PieceColor.Black);
        AddNewPiece<KnightPiece>(StartSquares.BlackKnightQ, PieceColor.Black);
        AddNewPiece<BishopPiece>(StartSquares.BlackBishopQ, PieceColor.Black);
        AddNewPiece<QueenPiece>(StartSquares.BlackQueen, PieceColor.Black);
        AddNewPiece<KingPiece>(StartSquares.BlackKing, PieceColor.Black);
        AddNewPiece<BishopPiece>(StartSquares.BlackBishopK, PieceColor.Black);
        AddNewPiece<KnightPiece>(StartSquares.BlackKnightK, PieceColor.Black);
        AddNewPiece<RookPiece>(StartSquares.BlackRookK, PieceColor.Black);

        AddNewPiece<PawnPiece>(StartSquares.BlackPawnA, PieceColor.Black);
        AddNewPiece<PawnPiece>(StartSquares.BlackPawnB, PieceColor.Black);
        AddNewPiece<PawnPiece>(StartSquares.BlackPawnC, PieceColor.Black);
        AddNewPiece<PawnPiece>(StartSquares.BlackPawnD, PieceColor.Black);
        AddNewPiece<PawnPiece>(StartSquares.BlackPawnE, PieceColor.Black);
        AddNewPiece<PawnPiece>(StartSquares.BlackPawnF, PieceColor.Black);
        AddNewPiece<PawnPiece>(StartSquares.BlackPawnG, PieceColor.Black);
        AddNewPiece<PawnPiece>(StartSquares.BlackPawnH, PieceColor.Black);
    }


    /// <summary>
    /// Adds a new piece to the Board of type <typeparamref name="T"/> at (row, col).
    /// New pieces should always be created in this way.
    /// </summary>
    /// <typeparam name="T">A child of the Piece abstract class</typeparam>
    /// <param name="row">The row index to place the new piece at.</param>
    /// <param name="col">The column index to place the new piece at.</param>
    /// <param name="color">The Color of the new piece.</param>
    /// <returns>The new piece of type <typeparamref name="T"/></returns>
    /// <exception cref="ArgumentException"></exception>
    public T AddNewPiece<T>(int row, int col, PieceColor color=PieceColor.White) where T : Piece
    {
        // Ensure row and col are in-bounds
        if (row < MinIndex || row > MaxIndex ||
            col < MinIndex || col > MaxIndex)
        {
            throw new ArgumentException("Row / column index out of range. " +
                $"Must be between {MinIndex} and {MaxIndex} (inclusive)");
        }

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
    /// Adds a new piece to the Board of type <typeparamref name="T"/> at the provided square.
    /// New pieces should always be created in this way.
    /// </summary>
    /// <typeparam name="T">A child of the Piece abstract class</typeparam>
    /// <param name="square">The (row, column) square to place the new piece at.</param>
    /// <param name="color">The Color of the new piece.</param>
    /// <returns>The new piece of type <typeparamref name="T"/></returns>
    public T AddNewPiece<T>((int row, int col) square, PieceColor color = PieceColor.White) where T : Piece
    {
        return AddNewPiece<T>(square.row, square.col, color);
    }


    // REFACTOR OUT LATER?
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
        else if (move.MoveType == MoveType.Castle)
        {
            Castle((CastleMove)move);
        }
    }


    /// <summary>
    /// Determines if a player will leave themself in check by moving the piece at "from" to "to".
    /// For En Passant, pass in the captured pawn's square as the "epCaptured" parameter.
    /// Castling does its own verification and is not passed through this method.
    /// </summary>
    /// <param name="from">The square the piece is moving from.</param>
    /// <param name="to">The square the piece is moving to.</param>
    /// <param name="epCaptured">If an En Passant move, the square of the captured pawn. Otherwise null</param>
    /// <returns>true if the player would be in check after the move. Otherwise, false</returns>
    /// <exception cref="ArgumentException"></exception>
    public bool MoveLeavesPlayerInCheck((int row, int col) from, (int row, int col) to, (int row, int col)? epCaptured = null)
    {
        // Ensure there the moving piece is not null
        IPiece? movingPiece = State[from.row, from.col];

        if (movingPiece == null)
        {
            throw new ArgumentException($"Piece not found at \"from\": (row: {from.row}, col: {from.col})");
        }
            
        // Return false when no king.
        if (Kings[movingPiece.Color] == null)
        {
            return false;
        }

        // Get target piece
        IPiece? capturedPiece = State[to.row, to.col];

        if (epCaptured != null)
        {
            // Overwrite capturedPiece for En Passant
            capturedPiece = State[epCaptured.Value.row, epCaptured.Value.col];

            // Set the "captured" square to null for En Passant
            State[epCaptured.Value.row, epCaptured.Value.col] = null;
        }

        // Make the move
        MovePiece(from, to);

        if (capturedPiece != null)
        {
            Pieces[capturedPiece.Color].Remove(capturedPiece);
        }

        // Record the result
        bool result = Kings[movingPiece.Color].IsChecked();

        // Roll back pieces
        MovePiece(to, from);
        
        if (capturedPiece != null)
        {
            if (epCaptured != null)
            {
                State[epCaptured.Value.row, epCaptured.Value.col] = capturedPiece;
            }
            else
            {
                State[to.row, to.col] = capturedPiece;
            }

            Pieces[capturedPiece.Color].Add(capturedPiece);
        }

        
        return result;
    }


    /// <summary>
    /// Updates the Board and involved pieces to reflect the StandardMove.
    /// The move is assumed to be valid.
    /// </summary>
    /// <param name="move">A StandardMove instance</param>
    public void StandardMove(StandardMove move)
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
    /// Updates the Board and involved pieces to reflect the EnPassantMove.
    /// The move is assumed to be valid.
    /// </summary>
    /// <param name="move">An EnPassantMove instance</param>
    public void EnPassant(EnPassantMove move)
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
    /// Updates the Board and involved pieces to reflect the PromotionMove.
    /// The move is assumed to be valid.
    /// </summary>
    /// <param name="move">A PromotionMove instance</param>
    /// <exception cref="ArgumentException"></exception>
    public void PawnPromote(PromotionMove move)
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


    /// <summary>
    /// Updates the Board and the involved king and rook to reflect the CastleMove.
    /// The move is assumed to be valid.
    /// </summary>
    /// <param name="move">A CastleMove instance</param>
    public void Castle(CastleMove move)
    {
        // Move king
        MovePiece(move.From, move.To);

        // Move Rook
        MovePiece(move.RookFrom, move.RookTo); 
    }

    #endregion



    #region Private Methods

    /// <summary>
    /// Updates the <see cref="State"/> to reflect the move. 
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