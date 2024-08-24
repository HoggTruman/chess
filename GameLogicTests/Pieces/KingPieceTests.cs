using FluentAssertions;
using GameLogic;
using GameLogic.Enums;
using GameLogic.Interfaces;
using GameLogic.Pieces;

namespace GameLogicTests.Pieces;

public class KingPieceTests
{
    #region IsChecked Tests

    [Fact]
    public void IsChecked_WithBlackUnderCheck_ReturnsTrue()
    {
        // Arrange
        Board testBoard = new();

        QueenPiece whiteQueen = new(0, 0, Color.White);
        KingPiece blackKing = new(7, 0, Color.Black);

        List<IPiece> pieces = [whiteQueen, blackKing];        
        testBoard.AddPieces(pieces);

        // Act
        var result = blackKing.IsChecked(testBoard);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsChecked_WithWhiteUnderCheck_ReturnsTrue()
    {
        Board testBoard = new();

        QueenPiece blackQueen = new(0, 0, Color.Black);
        KingPiece whiteKing = new(7, 0, Color.White);

        List<IPiece> pieces = [blackQueen, whiteKing];
        testBoard.AddPieces(pieces);

        // Act
        var result = whiteKing.IsChecked(testBoard);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsChecked_WithNoCheck_ReturnsFalse()
    {
        // Arrange 
        Board testBoard = new();

        KingPiece whiteKing = new(0, 0, Color.White);
        QueenPiece whiteQueen = new(1, 0, Color.White);
        KingPiece blackKing = new(0, 7, Color.Black);
        QueenPiece blackQueen = new(1, 7, Color.Black);       

        List<IPiece> pieces = [whiteKing, whiteQueen, blackKing, blackQueen];
        testBoard.AddPieces(pieces);

        // Act
        var whiteChecked = whiteKing.IsChecked(testBoard);
        var blackChecked = blackKing.IsChecked(testBoard);

        // Assert
        whiteChecked.Should().BeFalse();
        blackChecked.Should().BeFalse();
    }

    #endregion
}