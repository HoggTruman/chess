using GameLogic;
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
        Board testBoard = new();
        IPiece?[,] originalState = new IPiece?[Board.BoardSize, Board.BoardSize];

        QueenPiece testPiece = new(testBoard, 3, 3);
        originalState[3, 3] = testPiece;

        // Act
        IPiece?[,] copiedState = BoardHelpers.CopyState(originalState);

        // Assert
        Assert.True(originalState[testPiece.Row, testPiece.Col] == copiedState[testPiece.Row, testPiece.Col]);
        Assert.True(originalState != copiedState);
    }

    #endregion
}