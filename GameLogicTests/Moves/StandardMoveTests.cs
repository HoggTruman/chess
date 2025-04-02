using GameLogic;
using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Moves;
using GameLogic.Pieces;
using FluentAssertions;

namespace GameLogicTests.Moves;

public class StandardMoveTests
{
    #region Apply Tests

    [Fact]
    public void StandardMove_WithoutCapture_UpdatesBoardAndPiece()
    {
        // Arrange
        Board board = new();

        Square from = new(1, 1);
        Square to = new(2, 2);
        StandardMove move = new(from, to);

        var movingPiece = new QueenPiece(board, from, PieceColor.White);
        board.AddPiece(movingPiece);        

        // Act
        move.Apply(board);

        // Assert
        movingPiece.Square.Should().Be(to);
        board.At(from).Should().BeNull();
        board.At(to).Should().Be(movingPiece);
    }


    [Fact]
    public void StandardMove_WithCapture_UpdatesBoardAndPiece()
    {
        // Arrange
        Board board = new();

        Square from = new(1, 1);
        Square to = new(2, 2);

        var movingPiece = new QueenPiece(board, from, PieceColor.White);
        var capturedPiece = new QueenPiece(board, to, PieceColor.Black);
        board.AddPiece(movingPiece);
        board.AddPiece(capturedPiece);

        StandardMove move = new(from, to);

        // Act
        move.Apply(board);

        // Assert
        movingPiece.Square.Should().Be(to);
        board.At(from).Should().BeNull();
        board.At(to).Should().Be(movingPiece);
        board.Pieces[capturedPiece.Color].Should().BeEmpty();
    }

    #endregion


    #region LeavesPlayerInCheck Tests

    [Fact]
    public void LeavesPlayerInCheck_WithNoPieceAtFrom_ThrowsException()
    {
        // Arrange
        Board board = new();
        StandardMove move = new(new(0, 0), new(1, 0));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => move.LeavesPlayerInCheck(board));
    }


    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void LeavesPlayerInCheck_WithNoKing_ThrowsException(PieceColor playerColor)
    {
        // Arrange
        Board board = new();

        var queen = new QueenPiece(board, StartSquares.WhiteQueen, playerColor);
        board.AddPiece(queen);

        StandardMove move = new(queen.Square, new(queen.Row, queen.Col + 1));

        // Act + Assert
        Assert.Throws<Exception>(() => move.LeavesPlayerInCheck(board));
    }


    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void LeavesPlayerInCheck_WithNoEnemyPieces_ReturnsFalse(PieceColor playerColor)
    {
        // Arrange
        Board board = new();

        var king = new KingPiece(board, 0, 0, playerColor);
        var queen = new QueenPiece(board, 5, 5, playerColor);
        board.AddPiece(king);
        board.AddPiece(queen);

        StandardMove move = new(queen.Square, new(queen.Row, queen.Col + 1));

        // Act
        var result = move.LeavesPlayerInCheck(board);

        // Assert 
        result.Should().BeFalse();
    }


    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void LeavesPlayerInCheck_WhenNotLeftInCheck_ReturnsFalse(PieceColor playerColor)
    {
        // Arrange
        Board board = new();

        var playerKing = new KingPiece(board, 0, 0, playerColor);
        var playerQueen = new QueenPiece(board, 4, 5, playerColor);
        var enemyRook = new RookPiece(board, 7, 7, ColorHelpers.Opposite(playerColor));
        board.AddPiece(playerKing);
        board.AddPiece(playerQueen);
        board.AddPiece(enemyRook);

        StandardMove move = new(playerQueen.Square, new(5, 5));

        // Act
        var result = move.LeavesPlayerInCheck(board);

        // Assert 
        result.Should().BeFalse();
    }


    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void LeavesPlayerInCheck_WhenCheckBlocked_ReturnsFalse(PieceColor playerColor)
    {
        // Arrange
        Board board = new();

        var playerKing = new KingPiece(board, 0, 0, playerColor);
        var playerQueen = new QueenPiece(board, 7, 5, playerColor);
        var enemyRook = new RookPiece(board, 0, 7, ColorHelpers.Opposite(playerColor));
        board.AddPiece(playerKing);
        board.AddPiece(playerQueen);
        board.AddPiece(enemyRook);

        StandardMove move = new(playerQueen.Square, new(playerKing.Row, playerQueen.Col));

        // Act
        var result = move.LeavesPlayerInCheck(board);

        // Assert 
        result.Should().BeFalse();
    }


    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void LeavesPlayerInCheck_WhenLeftInCheck_ReturnsTrue(PieceColor playerColor)
    {
        // Arrange
        Board board = new();

        var playerKing = new KingPiece(board, 0, 0, playerColor);
        var enemyRook = new RookPiece(board, 1, 7, ColorHelpers.Opposite(playerColor));
        board.AddPiece(playerKing);
        board.AddPiece(enemyRook);

        StandardMove move = new(playerKing.Square, new(enemyRook.Row, playerKing.Col));

        // Act
        var result = move.LeavesPlayerInCheck(board);

        // Assert 
        result.Should().BeTrue();
    }


    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void LeavesPlayerInCheck_BoardIsTheSameBeforeAndAfter(PieceColor playerColor)
    {
        // In this test the player's queen will move to the square of the enemy's queen.
        // After calling the method, the board's State and Kings properties should be unchanged.
        // The Pieces lists should contain the same elements but dont't need to preserve order


        // Arrange
        Board board = new();

        var playerKing = new KingPiece(board, StartSquares.WhiteKing, playerColor);
        var playerQueen = new QueenPiece(board, 4, 4, playerColor);
        var enemyKing = new KingPiece(board, StartSquares.BlackKing, ColorHelpers.Opposite(playerColor));
        var enemyQueen = new QueenPiece(board, 3, 4, ColorHelpers.Opposite(playerColor));
        board.AddPiece(playerKing);
        board.AddPiece(playerQueen);
        board.AddPiece(enemyKing);
        board.AddPiece(enemyQueen);


        var playerKingBeforeSquare = playerKing.Square;
        var playerQueenBeforeSquare = playerQueen.Square;
        var enemyKingBeforeSquare = enemyKing.Square;
        var enemyQueenBeforeSquare = enemyQueen.Square;

        var whitePiecesBefore = board.Pieces[PieceColor.White].ToList();
        var blackPiecesBefore = board.Pieces[PieceColor.Black].ToList();

        var whiteKingBefore = board.GetKing(PieceColor.White);
        var blackKingBefore = board.GetKing(PieceColor.Black);

        var stateBefore = board.State.Clone();

        // Act
        StandardMove move = new(playerQueen.Square, enemyQueen.Square);
        move.LeavesPlayerInCheck(board);

        // Assert
        playerKing.Square.Should().Be(playerKingBeforeSquare);
        playerQueen.Square.Should().Be(playerQueenBeforeSquare);
        enemyKing.Square.Should().Be(enemyKingBeforeSquare);
        enemyQueen.Square.Should().Be(enemyQueenBeforeSquare);

        board.Pieces[PieceColor.White].Should().BeEquivalentTo(whitePiecesBefore);
        board.Pieces[PieceColor.Black].Should().BeEquivalentTo(blackPiecesBefore);

        board.GetKing(PieceColor.White).Should().Be(whiteKingBefore);
        board.GetKing(PieceColor.Black).Should().Be(blackKingBefore);

        board.State.Should().BeEquivalentTo(stateBefore, options => options.WithStrictOrdering());
    }

    #endregion
}
