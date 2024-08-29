using FluentAssertions;
using GameLogic;
using GameLogic.Enums;
using GameLogic.Interfaces;
using GameLogic.Moves;
using GameLogic.Pieces;

namespace GameLogicTests.Pieces;

public class PawnPieceTests
{
    #region GetEnPassantSquare Tests 

    [Fact]
    public void GetEnPassantSquare_WhenWhiteCanPerform_ReturnsSquare()
    {   
        // In this test, the black pawn starts on its starting square and advances two squares.
        // The white pawn starts on the correct row to perform En Passant.
        // The GetEnPassantSquare method should return the square where the white pawn lands after performing the move.

        // Arrange
        Board board = new();

        var whitePawn = board.AddNewPiece<PawnPiece>(4, 3, Color.White);
        var blackPawn = board.AddNewPiece<PawnPiece>(6, 4, Color.Black);

        (int row, int col) blackTo = (4, 4);
        StandardMove blackMove = new(blackPawn.Square, blackTo);

        (int row, int col) expectedTo = (5, 4);
        (int row, int col) expectedCaptured = blackTo;


        // Act
        board.HandleMove(blackMove);
        var result = whitePawn.GetEnPassantSquares();

        // Assert 
        Assert.NotNull(result);
        result.Value.to.Should().Be(expectedTo);
        result.Value.captured.Should().Be(expectedCaptured);
    }


    [Fact]
    public void GetEnPassantSquare_WhenBlackCanPerform_ReturnsSquare()
    {   
        // In this test, the white pawn starts on its starting square and advances two squares.
        // The black pawn starts on the correct row to perform En Passant.
        // The GetEnPassantSquare method should return the square where the black pawn lands after performing the move.

        // Arrange
        Board board = new();

        var whitePawn = board.AddNewPiece<PawnPiece>(1, 3, Color.White);
        var blackPawn = board.AddNewPiece<PawnPiece>(3, 4, Color.Black);

        (int row, int col) whiteTo = (3, 3);
        StandardMove whiteMove = new(whitePawn.Square, whiteTo);

        (int row, int col) expectedTo = (2, 3);
        (int row, int col) expectedCaptured = whiteTo;


        // Act
        board.HandleMove(whiteMove);
        var result = blackPawn.GetEnPassantSquares();

        // Assert 
        Assert.NotNull(result);
        result.Value.to.Should().Be(expectedTo);
        result.Value.captured.Should().Be(expectedCaptured);
    }


    [Fact]
    public void GetEnPassantSquare_WithoutMoveHistory_ReturnsNull()
    {   
        // In this test, the positioning is correct for white to perform en passant but there are no previous moves
        // The result should be null

        // Arrange
        Board board = new();

        var whitePawn = board.AddNewPiece<PawnPiece>(4, 3, Color.White);
        var blackPawn = board.AddNewPiece<PawnPiece>(4, 4, Color.Black);


        // Act
        var result = whitePawn.GetEnPassantSquares();

        // Assert 
        result.Should().BeNull();
    }


    [Fact]
    public void GetEnPassantSquare_WhenBlackAdvancesOneSquare_ReturnsNull()
    {   
        // In this test, the black pawn only advances one square to get in position
        // The result should be null

        // Arrange
        Board board = new();

        var whitePawn = board.AddNewPiece<PawnPiece>(4, 3, Color.White);
        var blackPawn = board.AddNewPiece<PawnPiece>(5, 4, Color.Black);

        (int row, int col) blackTo = (4, 4);
        StandardMove blackMove = new(blackPawn.Square, blackTo);

        // Act
        board.HandleMove(blackMove);
        var result = whitePawn.GetEnPassantSquares();

        // Assert 
        result.Should().BeNull();
    }


    [Fact]
    public void GetEnPassantSquare_WhenWhiteAdvancesOneSquare_ReturnsNull()
    {   
        // In this test, the white pawn only advances one square to get in position
        // The result should be null

        // Arrange
        Board board = new();

        var whitePawn = board.AddNewPiece<PawnPiece>(2, 3, Color.White);
        var blackPawn = board.AddNewPiece<PawnPiece>(3, 4, Color.Black);

        (int row, int col) whiteTo = (3, 3);
        StandardMove whiteMove = new(whitePawn.Square, whiteTo);

        // Act
        board.HandleMove(whiteMove);
        var result = blackPawn.GetEnPassantSquares();

        // Assert 
        result.Should().BeNull();
    }

    #endregion
}