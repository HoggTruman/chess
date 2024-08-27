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

        QueenPiece whiteQueen = new(testBoard, 0, 0, Color.White);
        KingPiece blackKing = new(testBoard, 7, 0, Color.Black);

        // Act
        var result = blackKing.IsChecked();

        // Assert
        result.Should().BeTrue();
    }


    [Fact]
    public void IsChecked_WithWhiteUnderCheck_ReturnsTrue()
    {
        Board testBoard = new();

        QueenPiece blackQueen = new(testBoard, 0, 0, Color.Black);
        KingPiece whiteKing = new(testBoard, 7, 0, Color.White);

        // Act
        var result = whiteKing.IsChecked();

        // Assert
        result.Should().BeTrue();
    }


    [Fact]
    public void IsChecked_WithNoCheck_ReturnsFalse()
    {
        // Arrange 
        Board testBoard = new();

        KingPiece whiteKing = new(testBoard, 0, 0, Color.White);
        QueenPiece whiteQueen = new(testBoard, 1, 0, Color.White);
        KingPiece blackKing = new(testBoard, 0, 7, Color.Black);
        QueenPiece blackQueen = new(testBoard, 1, 7, Color.Black);       

        // Act
        var whiteChecked = whiteKing.IsChecked();
        var blackChecked = blackKing.IsChecked();

        // Assert
        whiteChecked.Should().BeFalse();
        blackChecked.Should().BeFalse();
    }

    #endregion
}