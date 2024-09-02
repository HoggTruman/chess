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
        testBoard.Pieces[Color.White].Should().BeEmpty();
        testBoard.Pieces[Color.Black].Should().BeEmpty();
        testBoard.Kings[Color.White].Should().BeNull();
        testBoard.Kings[Color.Black].Should().BeNull();
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
        Color color = Color.White;

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
        Color color = Color.White;

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
        board.AddNewPiece<KingPiece>(0, 0, Color.White);

        // Act + Assert
        Assert.Throws<ArgumentException>(() => board.AddNewPiece<KingPiece>(1, 1, Color.White));
    }


    [Fact]
    public void AddNewPiece_ToOccupiedSquare_ThrowsArgumentException()
    {
        // Arrange
        Board board = new();
        board.AddNewPiece<QueenPiece>(0, 0, Color.White);

        // Act + Assert
        Assert.Throws<ArgumentException>(() => board.AddNewPiece<QueenPiece>(0, 0, Color.Black));
    }


    [Fact]
    public void AddNewPiece_RowIndexOutOfBounds_ThrowsArgumentException()
    {
        // Arrange
        Board board = new();

        // Act + Assert
        Assert.Throws<ArgumentException>(() => board.AddNewPiece<QueenPiece>(-1, 0, Color.Black));
        Assert.Throws<ArgumentException>(() => board.AddNewPiece<QueenPiece>(8, 0, Color.Black));
    }


    [Fact]
    public void AddNewPiece_ColIndexOutOfBounds_ThrowsArgumentException()
    {
        // Arrange
        Board board = new();

        // Act + Assert
        Assert.Throws<ArgumentException>(() => board.AddNewPiece<QueenPiece>(0, -1, Color.Black));
        Assert.Throws<ArgumentException>(() => board.AddNewPiece<QueenPiece>(0, 8, Color.Black));
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
    [InlineData(Color.White)]
    [InlineData(Color.Black)]
    public void MoveLeavesPlayerInCheck_WithNoKing_ReturnsFalse(Color playerColor)
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
    [InlineData(Color.White)]
    [InlineData(Color.Black)]
    public void MoveLeavesPlayerInCheck_WithNoEnemyPieces_ReturnsFalse(Color playerColor)
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
    [InlineData(Color.White)]
    [InlineData(Color.Black)]
    public void MoveLeavesPlayerInCheck_WhenNotLeftInCheck_ReturnsFalse(Color playerColor)
    {
        // Arrange
        Board board = new();

        var playerKing = board.AddNewPiece<KingPiece>(0, 0, playerColor);
        var playerQueen = board.AddNewPiece<QueenPiece>(5, 5, playerColor);

        var enemyRook = board.AddNewPiece<RookPiece>(7, 7, OppositeColor(playerColor));

        // Act
        var result = board.MoveLeavesPlayerInCheck(enemyRook.Square, (1, enemyRook.Col));

        // Assert 
        result.Should().BeFalse();
    }


    [Theory]
    [InlineData(Color.White)]
    [InlineData(Color.Black)]
    public void MoveLeavesPlayerInCheck_WhenCheckBlocked_ReturnsFalse(Color playerColor)
    {
        // Arrange
        Board board = new();

        var playerKing = board.AddNewPiece<KingPiece>(0, 0, playerColor);
        var playerQueen = board.AddNewPiece<QueenPiece>(0, 5, playerColor);

        var enemyRook = board.AddNewPiece<RookPiece>(7, 7, OppositeColor(playerColor));

        // Act
        var result = board.MoveLeavesPlayerInCheck(enemyRook.Square, (playerKing.Row, enemyRook.Col));

        // Assert 
        result.Should().BeFalse();
    }


    [Theory]
    [InlineData(Color.White)]
    [InlineData(Color.Black)]
    public void MoveLeavesPlayerInCheck_WhenLeftInCheck_ReturnsTrue(Color playerColor)
    {
        // Arrange
        Board board = new();

        var playerKing = board.AddNewPiece<KingPiece>(0, 0, playerColor);

        var enemyRook = board.AddNewPiece<RookPiece>(1, 7, OppositeColor(playerColor));

        // Act
        var result = board.MoveLeavesPlayerInCheck(playerKing.Square, (enemyRook.Row, playerKing.Col));

        // Assert 
        result.Should().BeTrue();
    }


    [Theory]
    [InlineData(Color.White)]
    [InlineData(Color.Black)]
    public void MoveLeavesPlayerInCheck_EnPassantLeavingInCheck_ReturnsTrue(Color playerColor)
    {
        // Arrange
        Board board = new();

        var playerKing = board.AddNewPiece<KingPiece>(4, 0, playerColor);
        var playerPawn = board.AddNewPiece<PawnPiece>(4, 3, playerColor);

        var enemyPawn = board.AddNewPiece<PawnPiece>(4, 2, OppositeColor(playerColor));
        var enemyRook = board.AddNewPiece<RookPiece>(4, 7, OppositeColor(playerColor));

        // Act
        var result = board.MoveLeavesPlayerInCheck(playerPawn.Square, (5, 2), enemyPawn.Square);

        // Assert 
        result.Should().BeTrue();
    }


    [Theory]
    [InlineData(Color.White)]
    [InlineData(Color.Black)]
    public void MoveLeavesPlayerInCheck_EnPassantNotLeavingInCheck_ReturnsFalse(Color playerColor)
    {
        // Arrange
        Board board = new();

        var playerKing = board.AddNewPiece<KingPiece>(4, 0, playerColor);
        var playerPawn = board.AddNewPiece<PawnPiece>(4, 3, playerColor);

        var enemyPawn = board.AddNewPiece<PawnPiece>(4, 2, OppositeColor(playerColor));

        // Act
        var result = board.MoveLeavesPlayerInCheck(playerPawn.Square, (5, 2), enemyPawn.Square);

        // Assert 
        result.Should().BeFalse();
    }


    [Theory]
    [InlineData(Color.White)]
    [InlineData(Color.Black)]
    public void MoveLeavesPlayerInCheck_BoardIsTheSameBeforeAndAfter(Color playerColor)
    {
        // In this test the player's queen will move to the square of the enemy's queen.
        // After calling the method, the board's State and Kings properties should be unchanged.
        // The Pieces lists should contain the same elements but dont't need to preserve order


        // Arrange
        Board board = new();

        var playerKing = board.AddNewPiece<KingPiece>(StartSquares.WhiteKing, playerColor);
        var playerQueen = board.AddNewPiece<QueenPiece>(4, 4, playerColor);
        
        var enemyKing = board.AddNewPiece<KingPiece>(StartSquares.BlackKing, OppositeColor(playerColor));
        var enemyQueen = board.AddNewPiece<QueenPiece>(3, 4, OppositeColor(playerColor));

        var playerKingBeforeSquare = playerKing.Square;
        var playerQueenBeforeSquare = playerQueen.Square;
        var enemyKingBeforeSquare = enemyKing.Square;
        var enemyQueenBeforeSquare = enemyQueen.Square;

        var whitePiecesBefore = board.Pieces[Color.White].ToList();
        var blackPiecesBefore = board.Pieces[Color.Black].ToList();

        var whiteKingBefore = board.Kings[Color.White];
        var blackKingBefore = board.Kings[Color.Black];

        var stateBefore = BoardHelpers.CopyState(board.State);

        // Act
        board.MoveLeavesPlayerInCheck(playerQueen.Square, enemyQueen.Square);

        // Assert
        playerKing.Square.Should().Be(playerKingBeforeSquare);
        playerQueen.Square.Should().Be(playerQueenBeforeSquare);
        enemyKing.Square.Should().Be(enemyKingBeforeSquare);
        enemyQueen.Square.Should().Be(enemyQueenBeforeSquare);

        board.Pieces[Color.White].Should().BeEquivalentTo(whitePiecesBefore);
        board.Pieces[Color.Black].Should().BeEquivalentTo(blackPiecesBefore);

        board.Kings[Color.White].Should().Be(whiteKingBefore);
        board.Kings[Color.Black].Should().Be(blackKingBefore);

        board.State.Should().BeEquivalentTo(stateBefore, options =>
            options.WithStrictOrdering());
    }


    [Theory]
    [InlineData(Color.White)]
    [InlineData(Color.Black)]
    public void MoveLeavesPlayerInCheck_BoardIsTheSameBeforeAndAfterEnPassant(Color playerColor)
    {
        // In this test the player's pawn will perform en passant
        // After calling the method, the board's State and Kings properties should be unchanged.
        // The Pieces lists should contain the same elements but dont't need to preserve order


        // Arrange
        Board board = new();

        var playerKing = board.AddNewPiece<KingPiece>(StartSquares.WhiteKing, playerColor);
        var playerPawn = board.AddNewPiece<PawnPiece>(4, 4, playerColor);
        
        var enemyKing = board.AddNewPiece<KingPiece>(StartSquares.BlackKing, OppositeColor(playerColor));
        var enemyPawn = board.AddNewPiece<PawnPiece>(4, 3, OppositeColor(playerColor));

        var playerKingBeforeSquare = playerKing.Square;
        var playerPawnBeforeSquare = playerPawn.Square;
        var enemyKingBeforeSquare = enemyKing.Square;
        var enemyPawnBeforeSquare = enemyPawn.Square;

        var whitePiecesBefore = board.Pieces[Color.White].ToList();
        var blackPiecesBefore = board.Pieces[Color.Black].ToList();

        var whiteKingBefore = board.Kings[Color.White];
        var blackKingBefore = board.Kings[Color.Black];

        var stateBefore = BoardHelpers.CopyState(board.State);

        // Act
        board.MoveLeavesPlayerInCheck(playerPawn.Square, (5, 3), enemyPawn.Square);

        // Assert
        playerKing.Square.Should().Be(playerKingBeforeSquare);
        playerPawn.Square.Should().Be(playerPawnBeforeSquare);
        enemyKing.Square.Should().Be(enemyKingBeforeSquare);
        enemyPawn.Square.Should().Be(enemyPawnBeforeSquare);

        board.Pieces[Color.White].Should().BeEquivalentTo(whitePiecesBefore);
        board.Pieces[Color.Black].Should().BeEquivalentTo(blackPiecesBefore);

        board.Kings[Color.White].Should().Be(whiteKingBefore);
        board.Kings[Color.Black].Should().Be(blackKingBefore);

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

        var movingPiece = board.AddNewPiece<QueenPiece>(from, Color.White);

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

        var movingPiece = board.AddNewPiece<QueenPiece>(from, Color.White);
        var capturedPiece = board.AddNewPiece<QueenPiece>(to, Color.Black);

        StandardMove move = new(from, to);

        // Act
        board.StandardMove(move);

        // Assert
        movingPiece.Square.Should().Be(to);
        board.State[from.row, from.col].Should().BeNull();
        board.State[to.row, to.col].Should().Be(movingPiece);
    }

    #endregion
}