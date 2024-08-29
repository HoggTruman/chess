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
        Board board = new();

        var whiteQueen = board.AddNewPiece<QueenPiece>(0, 0, Color.White);
        var blackKing = board.AddNewPiece<KingPiece>(7, 0, Color.Black);

        // Act
        var result = blackKing.IsChecked();

        // Assert
        result.Should().BeTrue();
    }


    [Fact]
    public void IsChecked_WithWhiteUnderCheck_ReturnsTrue()
    {
        Board board = new();

        var blackQueen = board.AddNewPiece<QueenPiece>(0, 0, Color.Black);
        var whiteKing = board.AddNewPiece<KingPiece>(7, 0, Color.White);

        // Act
        var result = whiteKing.IsChecked();

        // Assert
        result.Should().BeTrue();
    }


    [Fact]
    public void IsChecked_WithNoCheck_ReturnsFalse()
    {
        // Arrange 
        Board board = new();

        var whiteKing = board.AddNewPiece<KingPiece>(0, 0, Color.White);
        var whiteQueen = board.AddNewPiece<QueenPiece>(1, 0, Color.White);
        var blackKing = board.AddNewPiece<KingPiece>(0, 7, Color.Black);
        var blackQueen = board.AddNewPiece<QueenPiece>(1, 7, Color.Black);       

        // Act
        var whiteChecked = whiteKing.IsChecked();
        var blackChecked = blackKing.IsChecked();

        // Assert
        whiteChecked.Should().BeFalse();
        blackChecked.Should().BeFalse();
    }

    #endregion
}