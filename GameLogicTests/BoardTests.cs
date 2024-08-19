using FluentAssertions;
using GameLogic;
using GameLogic.Enums;
using GameLogic.Interfaces;
using GameLogic.Pieces;

namespace GameLogicTests;

public class BoardTests
{
    #region Constructor Tests

    [Fact]
    public void BoardConstructs_WithNoPieces()
    {
        // Arrange 
        Board testBoard = new();
        IPiece?[,] emptyBoardState = new IPiece?[Board.BoardSize, Board.BoardSize];

        // Assert
        testBoard.State.Should().BeEquivalentTo(emptyBoardState);
    }

    [Fact]
    public void BoardConstructs_WithPieces()
    {
        // Arrange
        QueenPiece queen = new(1, 1);
        PawnPiece pawn = new(4, 5);
        List<IPiece> pieces = [queen, pawn];

        Board testBoard = new(pieces);

        // Assert
        testBoard.State[1, 1].Should().BeEquivalentTo(queen);
        testBoard.State[4, 5].Should().BeEquivalentTo(pawn);
    }

    #endregion



    #region UnderCheck Tests

    [Fact]
    public void UnderCheck_WithBlackUnderCheck_ReturnsTrue()
    {
        // Arrange
        QueenPiece whiteQueen = new(0, 0, Color.White);
        KingPiece blackKing = new(7, 0, Color.Black);

        List<IPiece> pieces = [whiteQueen, blackKing];

        Board testBoard = new(pieces);

        // Act
        var result = testBoard.UnderCheck(blackKing.Color);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void UnderCheck_WithWhiteUnderCheck_ReturnsTrue()
    {
        QueenPiece blackQueen = new(0, 0, Color.Black);
        KingPiece whiteKing = new(7, 0, Color.White);

        List<IPiece> pieces = [blackQueen, whiteKing];

        Board testBoard = new(pieces);

        // Act
        var result = testBoard.UnderCheck(whiteKing.Color);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(Color.Black)]
    [InlineData(Color.White)]
    public void UnderCheck_WithNoCheck_ReturnsFalse(Color testColor)
    {
        KingPiece whiteKing = new(0, 0, Color.White);
        QueenPiece whiteQueen = new(1, 0, Color.White);
        KingPiece blackKing = new(0, 7, Color.Black);
        QueenPiece blackQueen = new(1, 7, Color.Black);       

        List<IPiece> pieces = [whiteKing, whiteQueen, blackKing, blackQueen];

        Board testBoard = new(pieces);

        // Act
        var result = testBoard.UnderCheck(testColor);

        // Assert
        result.Should().BeFalse();
    }

    #endregion
}