using FluentAssertions;
using GameLogic;
using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Interfaces;
using GameLogic.Moves;
using GameLogic.Pieces;
using static GameLogic.Helpers.ColorHelpers;

namespace GameLogicTests;

public class BoardTests
{
    #region Constructor Tests

    [Fact]
    public void BoardConstructs()
    {
        // Arrange 
        Board testBoard = new();
        IPiece?[,] emptyBoardState = new IPiece?[Board.BoardSize, Board.BoardSize];

        // Assert
        testBoard.State.Should().BeEquivalentTo(emptyBoardState);
        testBoard.Pieces[PieceColor.White].Should().BeEmpty();
        testBoard.Pieces[PieceColor.Black].Should().BeEmpty();
        testBoard.Kings[PieceColor.White].Should().BeNull();
        testBoard.Kings[PieceColor.Black].Should().BeNull();
    }

    #endregion



    #region AddNewPieceTests

    [Fact]
    public void AddNewPiece_ValidInput_ReturnsInitializedPiece()
    {
        // Arrange
        Board board = new();
        int row = 4;
        int col = 5;
        PieceColor color = PieceColor.White;

        // Act
        var resultPiece = board.AddNewPiece<QueenPiece>(row, col, color);

        // Assert
        resultPiece.Row.Should().Be(row);
        resultPiece.Col.Should().Be(col);
        resultPiece.Color.Should().Be(color);
        board.State[row, col].Should().NotBeNull();
        board.State[row, col].Should().Be(resultPiece);
    }


    [Fact]
    public void AddNewPiece_ValidSquareOverload_ReturnsInitializedPiece()
    {
        // Arrange
        Board board = new();
        (int row, int col) square = (4, 5);
        PieceColor color = PieceColor.White;

        // Act
        var resultPiece = board.AddNewPiece<QueenPiece>(square, color);

        // Assert
        resultPiece.Square.Should().Be(square);
        resultPiece.Color.Should().Be(color);
        board.State[square.row, square.col].Should().NotBeNull();
        board.State[square.row, square.col].Should().Be(resultPiece);
    }


    [Fact]
    public void AddNewPiece_TwoKingPiecesOfSameColor_ThrowsArgumentException()
    {
        // Arrange
        Board board = new();
        board.AddNewPiece<KingPiece>(0, 0, PieceColor.White);

        // Act + Assert
        Assert.Throws<ArgumentException>(() => board.AddNewPiece<KingPiece>(1, 1, PieceColor.White));
    }


    [Fact]
    public void AddNewPiece_ToOccupiedSquare_ThrowsArgumentException()
    {
        // Arrange
        Board board = new();
        board.AddNewPiece<QueenPiece>(0, 0, PieceColor.White);

        // Act + Assert
        Assert.Throws<ArgumentException>(() => board.AddNewPiece<QueenPiece>(0, 0, PieceColor.Black));
    }


    [Fact]
    public void AddNewPiece_RowIndexOutOfBounds_ThrowsArgumentException()
    {
        // Arrange
        Board board = new();

        // Act + Assert
        Assert.Throws<ArgumentException>(() => board.AddNewPiece<QueenPiece>(-1, 0, PieceColor.Black));
        Assert.Throws<ArgumentException>(() => board.AddNewPiece<QueenPiece>(8, 0, PieceColor.Black));
    }


    [Fact]
    public void AddNewPiece_ColIndexOutOfBounds_ThrowsArgumentException()
    {
        // Arrange
        Board board = new();

        // Act + Assert
        Assert.Throws<ArgumentException>(() => board.AddNewPiece<QueenPiece>(0, -1, PieceColor.Black));
        Assert.Throws<ArgumentException>(() => board.AddNewPiece<QueenPiece>(0, 8, PieceColor.Black));
    }

    #endregion



    #region MoveLeavesPlayerInCheck Tests

    [Fact]
    public void MoveLeavesPlayerInCheck_WithNoPieceAtFrom_ThrowsArgumentException()
    {
        // Arrange
        Board board = new();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => board.MoveLeavesPlayerInCheck((0, 0), (1, 0)));
    }


    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void MoveLeavesPlayerInCheck_WithNoKing_ReturnsFalse(PieceColor playerColor)
    {
        // Arrange
        Board board = new();

        var queen = board.AddNewPiece<QueenPiece>(StartSquares.WhiteQueen, playerColor);

        // Act
        var result = board.MoveLeavesPlayerInCheck(queen.Square, (queen.Row, queen.Col + 1));

        // Assert 
        result.Should().BeFalse();
    }


    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void MoveLeavesPlayerInCheck_WithNoEnemyPieces_ReturnsFalse(PieceColor playerColor)
    {
        // Arrange
        Board board = new();

        var king = board.AddNewPiece<KingPiece>(0, 0, playerColor);
        var queen = board.AddNewPiece<QueenPiece>(5, 5, playerColor);

        // Act
        var result = board.MoveLeavesPlayerInCheck(queen.Square, (queen.Row, queen.Col + 1));

        // Assert 
        result.Should().BeFalse();
    }


    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void MoveLeavesPlayerInCheck_WhenNotLeftInCheck_ReturnsFalse(PieceColor playerColor)
    {
        // Arrange
        Board board = new();

        var playerKing = board.AddNewPiece<KingPiece>(0, 0, playerColor);
        var playerQueen = board.AddNewPiece<QueenPiece>(5, 5, playerColor);

        var enemyRook = board.AddNewPiece<RookPiece>(7, 7, Opposite(playerColor));

        // Act
        var result = board.MoveLeavesPlayerInCheck(enemyRook.Square, (1, enemyRook.Col));

        // Assert 
        result.Should().BeFalse();
    }


    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void MoveLeavesPlayerInCheck_WhenCheckBlocked_ReturnsFalse(PieceColor playerColor)
    {
        // Arrange
        Board board = new();

        var playerKing = board.AddNewPiece<KingPiece>(0, 0, playerColor);
        var playerQueen = board.AddNewPiece<QueenPiece>(0, 5, playerColor);

        var enemyRook = board.AddNewPiece<RookPiece>(7, 7, Opposite(playerColor));

        // Act
        var result = board.MoveLeavesPlayerInCheck(enemyRook.Square, (playerKing.Row, enemyRook.Col));

        // Assert 
        result.Should().BeFalse();
    }


    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void MoveLeavesPlayerInCheck_WhenLeftInCheck_ReturnsTrue(PieceColor playerColor)
    {
        // Arrange
        Board board = new();

        var playerKing = board.AddNewPiece<KingPiece>(0, 0, playerColor);

        var enemyRook = board.AddNewPiece<RookPiece>(1, 7, Opposite(playerColor));

        // Act
        var result = board.MoveLeavesPlayerInCheck(playerKing.Square, (enemyRook.Row, playerKing.Col));

        // Assert 
        result.Should().BeTrue();
    }


    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void MoveLeavesPlayerInCheck_EnPassantLeavingInCheck_ReturnsTrue(PieceColor playerColor)
    {
        // Arrange
        Board board = new();

        var playerKing = board.AddNewPiece<KingPiece>(4, 0, playerColor);
        var playerPawn = board.AddNewPiece<PawnPiece>(4, 3, playerColor);

        var enemyPawn = board.AddNewPiece<PawnPiece>(4, 2, Opposite(playerColor));
        var enemyRook = board.AddNewPiece<RookPiece>(4, 7, Opposite(playerColor));

        // Act
        var result = board.MoveLeavesPlayerInCheck(playerPawn.Square, (5, 2), enemyPawn.Square);

        // Assert 
        result.Should().BeTrue();
    }


    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void MoveLeavesPlayerInCheck_EnPassantNotLeavingInCheck_ReturnsFalse(PieceColor playerColor)
    {
        // Arrange
        Board board = new();

        var playerKing = board.AddNewPiece<KingPiece>(4, 0, playerColor);
        var playerPawn = board.AddNewPiece<PawnPiece>(4, 3, playerColor);

        var enemyPawn = board.AddNewPiece<PawnPiece>(4, 2, Opposite(playerColor));

        // Act
        var result = board.MoveLeavesPlayerInCheck(playerPawn.Square, (5, 2), enemyPawn.Square);

        // Assert 
        result.Should().BeFalse();
    }


    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void MoveLeavesPlayerInCheck_BoardIsTheSameBeforeAndAfter(PieceColor playerColor)
    {
        // In this test the player's queen will move to the square of the enemy's queen.
        // After calling the method, the board's State and Kings properties should be unchanged.
        // The Pieces lists should contain the same elements but dont't need to preserve order


        // Arrange
        Board board = new();

        var playerKing = board.AddNewPiece<KingPiece>(StartSquares.WhiteKing, playerColor);
        var playerQueen = board.AddNewPiece<QueenPiece>(4, 4, playerColor);
        
        var enemyKing = board.AddNewPiece<KingPiece>(StartSquares.BlackKing, Opposite(playerColor));
        var enemyQueen = board.AddNewPiece<QueenPiece>(3, 4, Opposite(playerColor));

        var playerKingBeforeSquare = playerKing.Square;
        var playerQueenBeforeSquare = playerQueen.Square;
        var enemyKingBeforeSquare = enemyKing.Square;
        var enemyQueenBeforeSquare = enemyQueen.Square;

        var whitePiecesBefore = board.Pieces[PieceColor.White].ToList();
        var blackPiecesBefore = board.Pieces[PieceColor.Black].ToList();

        var whiteKingBefore = board.Kings[PieceColor.White];
        var blackKingBefore = board.Kings[PieceColor.Black];

        var stateBefore = BoardHelpers.CopyState(board.State);

        // Act
        board.MoveLeavesPlayerInCheck(playerQueen.Square, enemyQueen.Square);

        // Assert
        playerKing.Square.Should().Be(playerKingBeforeSquare);
        playerQueen.Square.Should().Be(playerQueenBeforeSquare);
        enemyKing.Square.Should().Be(enemyKingBeforeSquare);
        enemyQueen.Square.Should().Be(enemyQueenBeforeSquare);

        board.Pieces[PieceColor.White].Should().BeEquivalentTo(whitePiecesBefore);
        board.Pieces[PieceColor.Black].Should().BeEquivalentTo(blackPiecesBefore);

        board.Kings[PieceColor.White].Should().Be(whiteKingBefore);
        board.Kings[PieceColor.Black].Should().Be(blackKingBefore);

        board.State.Should().BeEquivalentTo(stateBefore, options =>
            options.WithStrictOrdering());
    }


    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void MoveLeavesPlayerInCheck_BoardIsTheSameBeforeAndAfterEnPassant(PieceColor playerColor)
    {
        // In this test the player's pawn will perform en passant
        // After calling the method, the board's State and Kings properties should be unchanged.
        // The Pieces lists should contain the same elements but dont't need to preserve order


        // Arrange
        Board board = new();

        var playerKing = board.AddNewPiece<KingPiece>(StartSquares.WhiteKing, playerColor);
        var playerPawn = board.AddNewPiece<PawnPiece>(4, 4, playerColor);
        
        var enemyKing = board.AddNewPiece<KingPiece>(StartSquares.BlackKing, Opposite(playerColor));
        var enemyPawn = board.AddNewPiece<PawnPiece>(4, 3, Opposite(playerColor));

        var playerKingBeforeSquare = playerKing.Square;
        var playerPawnBeforeSquare = playerPawn.Square;
        var enemyKingBeforeSquare = enemyKing.Square;
        var enemyPawnBeforeSquare = enemyPawn.Square;

        var whitePiecesBefore = board.Pieces[PieceColor.White].ToList();
        var blackPiecesBefore = board.Pieces[PieceColor.Black].ToList();

        var whiteKingBefore = board.Kings[PieceColor.White];
        var blackKingBefore = board.Kings[PieceColor.Black];

        var stateBefore = BoardHelpers.CopyState(board.State);

        // Act
        board.MoveLeavesPlayerInCheck(playerPawn.Square, (5, 3), enemyPawn.Square);

        // Assert
        playerKing.Square.Should().Be(playerKingBeforeSquare);
        playerPawn.Square.Should().Be(playerPawnBeforeSquare);
        enemyKing.Square.Should().Be(enemyKingBeforeSquare);
        enemyPawn.Square.Should().Be(enemyPawnBeforeSquare);

        board.Pieces[PieceColor.White].Should().BeEquivalentTo(whitePiecesBefore);
        board.Pieces[PieceColor.Black].Should().BeEquivalentTo(blackPiecesBefore);

        board.Kings[PieceColor.White].Should().Be(whiteKingBefore);
        board.Kings[PieceColor.Black].Should().Be(blackKingBefore);

        board.State.Should().BeEquivalentTo(stateBefore, options =>
            options.WithStrictOrdering());
    }

    #endregion



    #region StandardMove Tests

    [Fact]
    public void StandardMove_WithoutCapture_UpdatesBoardAndPiece()
    {
        // Arrange
        Board board = new();

        (int row, int col) from = (1, 1);
        (int row, int col) to = (2, 2);

        var movingPiece = board.AddNewPiece<QueenPiece>(from, PieceColor.White);

        StandardMove move = new(from, to);

        // Act
        board.StandardMove(move);

        // Assert
        movingPiece.Square.Should().Be(to);
        board.State[from.row, from.col].Should().BeNull();
        board.State[to.row, to.col].Should().Be(movingPiece);
    }


    [Fact]
    public void StandardMove_WithCapture_UpdatesBoardAndPiece()
    {
        // Arrange
        Board board = new();

        (int row, int col) from = (1, 1);
        (int row, int col) to = (2, 2);

        var movingPiece = board.AddNewPiece<QueenPiece>(from, PieceColor.White);
        var capturedPiece = board.AddNewPiece<QueenPiece>(to, PieceColor.Black);

        StandardMove move = new(from, to);

        // Act
        board.StandardMove(move);

        // Assert
        movingPiece.Square.Should().Be(to);
        board.State[from.row, from.col].Should().BeNull();
        board.State[to.row, to.col].Should().Be(movingPiece);
    }

    #endregion



    #region EnPassant Tests

    [Fact]
    public void EnPassant_UpdatesBoardAndPiece()
    {
        // Arrange
        Board board = new();

        (int row, int col) from = (4, 4);
        (int row, int col) to = (5, 3);
        (int row, int col) captured = (4, 3);

        var movingPawn = board.AddNewPiece<PawnPiece>(from, PieceColor.White);
        var capturedPawn = board.AddNewPiece<PawnPiece>(captured, PieceColor.Black);

        EnPassantMove move = new(from, to, captured);

        // Act
        board.EnPassant(move);

        // Assert
        movingPawn.Square.Should().Be(to);
        board.State[from.row, from.col].Should().BeNull();
        board.State[to.row, to.col].Should().Be(movingPawn);
        board.State[captured.row, captured.col].Should().BeNull();
    }

    #endregion



    #region PawnPromote Tests

    [Fact]
    public void PawnPromote_WithoutCapture_UpdatesBoardAndCreatesPiece()
    {
        // Arrange
        Board board = new();

        (int row, int col) from = (6, 4);
        (int row, int col) to = (7, 4);

        var movingPawn = board.AddNewPiece<PawnPiece>(from, PieceColor.White);

        PromotionMove move = new(from, to, PieceType.Queen);

        // Act
        board.PawnPromote(move);
        var promotedPiece = board.State[to.row, to.col];

        // Assert
        Assert.NotNull(promotedPiece);
        promotedPiece.Square.Should().Be(to);
    }


    [Fact]
    public void PawnPromote_WitCapture_UpdatesBoardAndCreatesPiece()
    {
        // Arrange
        Board board = new();

        (int row, int col) from = (6, 4);
        (int row, int col) to = (7, 5);

        var movingPawn = board.AddNewPiece<PawnPiece>(from, PieceColor.White);
        var capturedPiece = board.AddNewPiece<QueenPiece>(to, PieceColor.Black);

        PromotionMove move = new(from, to, PieceType.Queen);

        // Act
        board.PawnPromote(move);
        var promotedPiece = board.State[to.row, to.col];

        // Assert
        Assert.NotNull(promotedPiece);
        board.State[to.row, to.col].Should().NotBe(capturedPiece);
        promotedPiece.Square.Should().Be(to);
    }


    [Theory]
    [InlineData(PieceColor.Black)]
    [InlineData(PieceColor.White)]
    public void PawnPromote_CreatesPieceOfSameColor(PieceColor color)
    {
        // Arrange
        Board board = new();

        (int row, int col) from = (6, 4);
        (int row, int col) to = (7, 4);

        var movingPawn = board.AddNewPiece<PawnPiece>(from, color);

        PromotionMove move = new(from, to, PieceType.Queen);

        // Act
        board.PawnPromote(move);
        var promotedPiece = board.State[to.row, to.col];

        // Assert
        Assert.NotNull(promotedPiece);
        promotedPiece.Color.Should().Be(color);
    }


    [Fact]
    public void PawnPromote_PromotedToQueen_CreatesQueenPiece()
    {
        // Arrange
        Board board = new();

        (int row, int col) from = (6, 4);
        (int row, int col) to = (7, 4);

        var movingPawn = board.AddNewPiece<PawnPiece>(from, PieceColor.White);

        PromotionMove move = new(from, to, PieceType.Queen);

        // Act
        board.PawnPromote(move);
        var promotedPiece = board.State[to.row, to.col];

        // Assert
        Assert.NotNull(promotedPiece);
        promotedPiece.GetType().Should().Be(typeof(QueenPiece));
    }


    [Fact]
    public void PawnPromote_PromotedToRook_CreatesRookPiece()
    {
        // Arrange
        Board board = new();

        (int row, int col) from = (6, 4);
        (int row, int col) to = (7, 4);

        var movingPawn = board.AddNewPiece<PawnPiece>(from, PieceColor.White);

        PromotionMove move = new(from, to, PieceType.Rook);

        // Act
        board.PawnPromote(move);
        var promotedPiece = board.State[to.row, to.col];

        // Assert
        Assert.NotNull(promotedPiece);
        promotedPiece.GetType().Should().Be(typeof(RookPiece));
    }


    [Fact]
    public void PawnPromote_PromotedToBishop_CreatesBishopPiece()
    {
        // Arrange
        Board board = new();

        (int row, int col) from = (6, 4);
        (int row, int col) to = (7, 4);

        var movingPawn = board.AddNewPiece<PawnPiece>(from, PieceColor.White);

        PromotionMove move = new(from, to, PieceType.Bishop);

        // Act
        board.PawnPromote(move);
        var promotedPiece = board.State[to.row, to.col];

        // Assert
        Assert.NotNull(promotedPiece);
        promotedPiece.GetType().Should().Be(typeof(BishopPiece));
    }


    [Fact]
    public void PawnPromote_PromotedToKnight_CreatesKnightPiece()
    {
        // Arrange
        Board board = new();

        (int row, int col) from = (6, 4);
        (int row, int col) to = (7, 4);

        var movingPawn = board.AddNewPiece<PawnPiece>(from, PieceColor.White);

        PromotionMove move = new(from, to, PieceType.Knight);

        // Act
        board.PawnPromote(move);
        var promotedPiece = board.State[to.row, to.col];

        // Assert
        Assert.NotNull(promotedPiece);
        promotedPiece.GetType().Should().Be(typeof(KnightPiece));
    }



    #endregion



    #region Castle Tests

    [Fact]
    public void Castle_UpdatesBoardAndPieces()
    {
        // Arrange 
        Board board = new();

        (int row, int col) kingFrom = (0, 4);
        (int row, int col) kingTo = (0, 6);
        (int row, int col) rookFrom = (0, 7);
        (int row, int col) rookTo = (0, 5);

        var king = board.AddNewPiece<KingPiece>(kingFrom, PieceColor.White);
        var rook = board.AddNewPiece<RookPiece>(rookFrom, PieceColor.White);

        CastleMove move = new(kingFrom, kingTo, rookFrom, rookTo);

        // Act
        board.Castle(move);

        // Assert
        king.Square.Should().Be(kingTo);
        rook.Square.Should().Be(rookTo);
        board.State[kingFrom.row, kingFrom.col].Should().BeNull();
        board.State[kingTo.row, kingTo.col].Should().Be(king);
        board.State[rookFrom.row, rookFrom.col].Should().BeNull();
        board.State[rookTo.row, rookTo.col].Should().Be(rook);
    }

    #endregion
}