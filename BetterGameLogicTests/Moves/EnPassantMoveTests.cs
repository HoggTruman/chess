using BetterGameLogic;
using BetterGameLogic.Constants;
using BetterGameLogic.Enums;
using BetterGameLogic.Helpers;
using BetterGameLogic.Moves;
using BetterGameLogic.Pieces;
using FluentAssertions;

namespace BetterGameLogicTests.Moves;

public class EnPassantMoveTests
{
    #region Apply Tests

    [Fact]
    public void Apply_UpdatesBoardAndPiece()
    {
        // Arrange
        Board board = new();

        Square from = new(4, 4);
        Square to = new(5, 3);
        Square captured = new(4, 3);

        var movingPawn = new PawnPiece(board, from, PieceColor.White);
        var capturedPawn = new PawnPiece(board, captured, PieceColor.Black);
        board.AddPiece(movingPawn);
        board.AddPiece(capturedPawn);

        EnPassantMove move = new(from, to);

        // Act
        move.Apply(board);

        // Assert
        movingPawn.Square.Should().Be(to);
        board.At(from).Should().BeNull();
        board.At(to).Should().Be(movingPawn);
        board.At(captured).Should().BeNull();
        board.Pieces[capturedPawn.Color].Should().BeEmpty();
    }

    #endregion



    #region LeavesPlayerInCheck Tests

    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void LeavesPlayerInCheck_EnPassantLeavingInCheck_ReturnsTrue(PieceColor playerColor)
    {
        // Arrange
        Board board = new();

        var playerKing = new KingPiece(board, 4, 0, playerColor);
        var playerPawn = new PawnPiece(board, 4, 3, playerColor);
        var enemyPawn = new PawnPiece(board, 4, 2, ColorHelpers.Opposite(playerColor));
        var enemyRook = new RookPiece(board, 4, 7, ColorHelpers.Opposite(playerColor));
        board.AddPiece(playerKing);
        board.AddPiece(playerPawn);
        board.AddPiece(enemyPawn);
        board.AddPiece(enemyRook);

        EnPassantMove enPassantMove = new(playerPawn.Square, new(5, 2));

        // Act
        var result = enPassantMove.LeavesPlayerInCheck(board);

        // Assert 
        result.Should().BeTrue();
    }


    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void LeavesPlayerInCheck_EnPassantNotLeavingInCheck_ReturnsFalse(PieceColor playerColor)
    {
        // Arrange
        Board board = new();

        var playerKing = new KingPiece(board, 4, 0, playerColor);
        var playerPawn = new PawnPiece(board, 4, 3, playerColor);
        var enemyPawn = new PawnPiece(board, 4, 2, ColorHelpers.Opposite(playerColor));
        board.AddPiece(playerKing);
        board.AddPiece(playerPawn);
        board.AddPiece(enemyPawn);

        EnPassantMove enPassantMove = new(playerPawn.Square, new(5, 2));

        // Act
        var result = enPassantMove.LeavesPlayerInCheck(board);

        // Assert 
        result.Should().BeFalse();
    }


    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void LeavesPlayerInCheck_BoardIsTheSameBeforeAndAfterEnPassant(PieceColor playerColor)
    {
        // In this test the player's pawn will perform en passant
        // After calling the method, the board's State and Kings properties should be unchanged.
        // The Pieces lists should contain the same elements but dont't need to preserve order


        // Arrange
        Board board = new();

        var playerKing = new KingPiece(board, StartSquares.WhiteKing, playerColor);
        var playerPawn = new PawnPiece(board, 4, 4, playerColor);
        var enemyKing = new KingPiece(board, StartSquares.BlackKing, ColorHelpers.Opposite(playerColor));
        var enemyPawn = new PawnPiece(board, 4, 3, ColorHelpers.Opposite(playerColor));
        board.AddPiece(playerKing);
        board.AddPiece(playerPawn);
        board.AddPiece(enemyKing);
        board.AddPiece(enemyPawn);

        var playerKingBeforeSquare = playerKing.Square;
        var playerPawnBeforeSquare = playerPawn.Square;
        var enemyKingBeforeSquare = enemyKing.Square;
        var enemyPawnBeforeSquare = enemyPawn.Square;

        var whitePiecesBefore = board.Pieces[PieceColor.White].ToList();
        var blackPiecesBefore = board.Pieces[PieceColor.Black].ToList();

        var whiteKingBefore = board.GetKing(PieceColor.White);
        var blackKingBefore = board.GetKing(PieceColor.Black);

        var stateBefore = board.State.Clone();

        // Act
        EnPassantMove enPassantMove = new(playerPawn.Square, new(5, 3));
        enPassantMove.LeavesPlayerInCheck(board);

        // Assert
        playerKing.Square.Should().Be(playerKingBeforeSquare);
        playerPawn.Square.Should().Be(playerPawnBeforeSquare);
        enemyKing.Square.Should().Be(enemyKingBeforeSquare);
        enemyPawn.Square.Should().Be(enemyPawnBeforeSquare);

        board.Pieces[PieceColor.White].Should().BeEquivalentTo(whitePiecesBefore);
        board.Pieces[PieceColor.Black].Should().BeEquivalentTo(blackPiecesBefore);

        board.GetKing(PieceColor.White).Should().Be(whiteKingBefore);
        board.GetKing(PieceColor.Black).Should().Be(blackKingBefore);

        board.State.Should().BeEquivalentTo(stateBefore, options => options.WithStrictOrdering());
    }

    #endregion
}
