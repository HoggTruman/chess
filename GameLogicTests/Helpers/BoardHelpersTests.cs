using FluentAssertions;
using GameLogic;
using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Interfaces;
using GameLogic.Pieces;

namespace GameLogicTests.Helpers;

public class BoardHelperTests
{
    #region CopyState Tests

    [Fact]
    public void CopyState_ReturnsIndependentArray()
    {
        // Arrange
        Board board = new();
        QueenPiece testPiece = board.AddNewPiece<QueenPiece>(3, 3, PieceColor.White);
        IPiece?[,] originalState = board.State;

        // Act
        IPiece?[,] copiedState = BoardHelpers.CopyState(originalState);

        // Assert
        Assert.True(originalState[testPiece.Row, testPiece.Col] == copiedState[testPiece.Row, testPiece.Col]);
        Assert.True(originalState != copiedState);
    }

    #endregion



    #region SquareIsInBounds Tests

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 7)]
    [InlineData(7, 0)]
    [InlineData(7, 7)]
    [InlineData(0, 4)]
    [InlineData(4, 0)]
    [InlineData(7, 4)]
    [InlineData(4, 7)]
    [InlineData(1, 2)]
    [InlineData(5, 6)]
    [InlineData(4, 4)]
    public void SquaresIsInBounds_WithInBoundsSquare_ReturnsTrue(int row, int col)
    {
        // Arrange
        (int row, int col) square = (row, col);

        // Act
        var result = BoardHelpers.SquareIsInBounds(square);

        // Assert
        result.Should().BeTrue();
    }


    [Theory]
    [InlineData(-1, -1)]
    [InlineData(-1, 7)]
    [InlineData(8, -1)]
    [InlineData(8, 8)]
    [InlineData(-1, 4)]
    [InlineData(4, -1)]
    [InlineData(8, 4)]
    [InlineData(4, 8)]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(-5, 12)]
    public void SquaresIsInBounds_WithOutOfBoundsSquare_ReturnsFalse(int row, int col)
    {
        // Arrange
        (int row, int col) square = (row, col);

        // Act
        var result = BoardHelpers.SquareIsInBounds(square);

        // Assert
        result.Should().BeFalse();
    }

    #endregion



    #region RotateSquare180 Tests

    [Theory]
    [InlineData(0, 0, 7, 7)]
    [InlineData(7, 7, 0, 0)]
    [InlineData(0, 7, 7, 0)]
    [InlineData(7, 0, 0, 7)]
    [InlineData(2, 4, 5, 3)]
    [InlineData(5, 3, 2, 4)]
    [InlineData(4, 4, 3, 3)]
    [InlineData(3, 3, 4, 4)]
    public void RotateSquare180_ReturnsCorrectSquare(int inRow, int inCol, int outRow, int outCol)
    {
        // Arrange
        (int row, int col) input = (inRow, inCol);
        (int row, int col) expected = (outRow, outCol);

        // Act
        var result = BoardHelpers.RotateSquare180(input);

        // Assert
        result.Should().Be(expected);
    }

    #endregion
}