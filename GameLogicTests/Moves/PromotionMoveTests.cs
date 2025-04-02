using GameLogic;
using GameLogic.Enums;
using GameLogic.Moves;
using GameLogic.Pieces;
using FluentAssertions;

namespace GameLogicTests.Moves;

public class PromotionMoveTests
{
    #region Apply Tests

    [Fact]
    public void Apply_WithoutCapture_UpdatesBoardAndCreatesPiece()
    {
        // Arrange
        Board board = new();

        Square from = new(6, 4);
        Square to = new(7, 4);

        var movingPawn = new PawnPiece(board, from, PieceColor.White);
        board.AddPiece(movingPawn);

        PromotionMove move = new(from, to, PieceType.Queen);

        // Act
        move.Apply(board);
        var promotedPiece = board.At(to);

        // Assert
        Assert.NotNull(promotedPiece);
        promotedPiece.Square.Should().Be(to);
        board.At(from).Should().BeNull();
        board.Pieces[movingPawn.Color].Should().Contain(promotedPiece);
    }


    [Fact]
    public void Apply_WitCapture_UpdatesBoardAndCreatesPiece()
    {
        // Arrange
        Board board = new();

        Square from = new(6, 4);
        Square to = new(7, 5);

        var movingPawn = new PawnPiece(board, from, PieceColor.White);
        var capturedPiece = new QueenPiece(board, to, PieceColor.Black);
        board.AddPiece(movingPawn);
        board.AddPiece(capturedPiece);

        PromotionMove move = new(from, to, PieceType.Queen);

        // Act
        move.Apply(board);
        var promotedPiece = board.At(to);

        // Assert
        Assert.NotNull(promotedPiece);
        board.At(to).Should().NotBe(capturedPiece);
        promotedPiece.Square.Should().Be(to);
        board.At(from).Should().BeNull();
        board.Pieces[movingPawn.Color].Should().Contain(promotedPiece);
        board.Pieces[capturedPiece.Color].Should().NotContain(capturedPiece);
    }


    [Theory]
    [InlineData(PieceColor.Black)]
    [InlineData(PieceColor.White)]
    public void Apply_CreatesPieceOfSameColor(PieceColor color)
    {
        // Arrange
        Board board = new();

        Square from = new(6, 4);
        Square to = new(7, 4);

        var movingPawn = new PawnPiece(board, from, color);
        board.AddPiece(movingPawn);

        PromotionMove move = new(from, to, PieceType.Queen);

        // Act
        move.Apply(board);
        var promotedPiece = board.At(to);

        // Assert
        Assert.NotNull(promotedPiece);
        promotedPiece.Color.Should().Be(color);
    }


    [Fact]
    public void Apply_PromotedToQueen_CreatesQueenPiece()
    {
        // Arrange
        Board board = new();

        Square from = new(6, 4);
        Square to = new(7, 4);

        var movingPawn = new PawnPiece(board, from, PieceColor.White);
        board.AddPiece(movingPawn);

        PromotionMove move = new(from, to, PieceType.Queen);

        // Act
        move.Apply(board);
        var promotedPiece = board.At(to);

        // Assert
        Assert.NotNull(promotedPiece);
        promotedPiece.GetType().Should().Be(typeof(QueenPiece));
    }


    [Fact]
    public void Apply_PromotedToRook_CreatesRookPiece()
    {
        // Arrange
        Board board = new();

        Square from = new(6, 4);
        Square to = new(7, 4);

        var movingPawn = new PawnPiece(board, from, PieceColor.White);
        board.AddPiece(movingPawn);

        PromotionMove move = new(from, to, PieceType.Rook);

        // Act
        move.Apply(board);
        var promotedPiece = board.At(to);

        // Assert
        Assert.NotNull(promotedPiece);
        promotedPiece.GetType().Should().Be(typeof(RookPiece));
    }


    [Fact]
    public void Apply_PromotedToBishop_CreatesBishopPiece()
    {
        // Arrange
        Board board = new();

        Square from = new(6, 4);
        Square to = new(7, 4);

        var movingPawn = new PawnPiece(board, from, PieceColor.White);
        board.AddPiece(movingPawn);

        PromotionMove move = new(from, to, PieceType.Bishop);

        // Act
        move.Apply(board);
        var promotedPiece = board.At(to);

        // Assert
        Assert.NotNull(promotedPiece);
        promotedPiece.GetType().Should().Be(typeof(BishopPiece));
    }


    [Fact]
    public void Apply_PromotedToKnight_CreatesKnightPiece()
    {
        // Arrange
        Board board = new();

        Square from = new(6, 4);
        Square to = new(7, 4);

        var movingPawn = new PawnPiece(board, from, PieceColor.White);
        board.AddPiece(movingPawn);

        PromotionMove move = new(from, to, PieceType.Knight);

        // Act
        move.Apply(board);
        var promotedPiece = board.At(to);

        // Assert
        Assert.NotNull(promotedPiece);
        promotedPiece.GetType().Should().Be(typeof(KnightPiece));
    }

    #endregion
}
