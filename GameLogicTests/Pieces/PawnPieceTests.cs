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

        PawnPiece whitePawn = new(4, 3, Color.White);
        PawnPiece blackPawn = new(6, 4, Color.Black);

        List<IPiece> pieces = [whitePawn, blackPawn];

        testBoard.AddPieces(pieces);


        (int row, int col) blackTo = (4, 4);
        Move blackMove = new(blackPawn.Square, blackTo);

        (int row, int col) expectedTo = (5, 4);
        (int row, int col) expectedCaptured = blackTo;


        // Act
        testBoard.HandleMove(blackMove);
        var result = whitePawn.GetEnPassantSquares(testBoard);

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

        PawnPiece whitePawn = new(1, 3, Color.White);
        PawnPiece blackPawn = new(3, 4, Color.Black);

        List<IPiece> pieces = [whitePawn, blackPawn];

        testBoard.AddPieces(pieces);


        (int row, int col) whiteTo = (3, 3);
        Move whiteMove = new(whitePawn.Square, whiteTo);

        (int row, int col) expectedTo = (2, 3);
        (int row, int col) expectedCaptured = whiteTo;


        // Act
        testBoard.HandleMove(whiteMove);
        var result = blackPawn.GetEnPassantSquares(testBoard);

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

        PawnPiece whitePawn = new(4, 3, Color.White);
        PawnPiece blackPawn = new(4, 4, Color.Black);

        List<IPiece> pieces = [whitePawn, blackPawn];

        testBoard.AddPieces(pieces);

        // Act
        var result = whitePawn.GetEnPassantSquares(testBoard);

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

        PawnPiece whitePawn = new(4, 3, Color.White);
        PawnPiece blackPawn = new(5, 4, Color.Black);

        List<IPiece> pieces = [whitePawn, blackPawn];

        (int row, int col) blackTo = (4, 4);
        Move blackMove = new(blackPawn.Square, blackTo);

        testBoard.AddPieces(pieces);

        // Act
        testBoard.HandleMove(blackMove);
        var result = whitePawn.GetEnPassantSquares(testBoard);

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

        PawnPiece whitePawn = new(2, 3, Color.White);
        PawnPiece blackPawn = new(3, 4, Color.Black);

        List<IPiece> pieces = [whitePawn, blackPawn];

        testBoard.AddPieces(pieces);


        (int row, int col) whiteTo = (3, 3);
        Move whiteMove = new(whitePawn.Square, whiteTo);

        // Act
        testBoard.HandleMove(whiteMove);
        var result = blackPawn.GetEnPassantSquares(testBoard);

        // Assert 
        result.Should().BeNull();
    }

    #endregion
}