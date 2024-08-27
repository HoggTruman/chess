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
        Board testBoard = new();

        PawnPiece whitePawn = new(testBoard, 4, 3, Color.White);
        PawnPiece blackPawn = new(testBoard, 6, 4, Color.Black);

        (int row, int col) blackTo = (4, 4);
        StandardMove blackMove = new(blackPawn.Square, blackTo);

        (int row, int col) expectedTo = (5, 4);
        (int row, int col) expectedCaptured = blackTo;


        // Act
        testBoard.HandleMove(blackMove);
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
        Board testBoard = new();

        PawnPiece whitePawn = new(testBoard, 1, 3, Color.White);
        PawnPiece blackPawn = new(testBoard, 3, 4, Color.Black);

        (int row, int col) whiteTo = (3, 3);
        StandardMove whiteMove = new(whitePawn.Square, whiteTo);

        (int row, int col) expectedTo = (2, 3);
        (int row, int col) expectedCaptured = whiteTo;


        // Act
        testBoard.HandleMove(whiteMove);
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
        Board testBoard = new();

        PawnPiece whitePawn = new(testBoard, 4, 3, Color.White);
        PawnPiece blackPawn = new(testBoard, 4, 4, Color.Black);


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
        Board testBoard = new();

        PawnPiece whitePawn = new(testBoard, 4, 3, Color.White);
        PawnPiece blackPawn = new(testBoard, 5, 4, Color.Black);

        (int row, int col) blackTo = (4, 4);
        StandardMove blackMove = new(blackPawn.Square, blackTo);

        // Act
        testBoard.HandleMove(blackMove);
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
        Board testBoard = new();

        PawnPiece whitePawn = new(testBoard, 2, 3, Color.White);
        PawnPiece blackPawn = new(testBoard, 3, 4, Color.Black);

        (int row, int col) whiteTo = (3, 3);
        StandardMove whiteMove = new(whitePawn.Square, whiteTo);

        // Act
        testBoard.HandleMove(whiteMove);
        var result = blackPawn.GetEnPassantSquares();

        // Assert 
        result.Should().BeNull();
    }

    #endregion
}