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
        Board board = new();
        QueenPiece testPiece = board.AddNewPiece<QueenPiece>(3, 3);
        IPiece?[,] originalState = board.State;

        // Act
        IPiece?[,] copiedState = BoardHelpers.CopyState(originalState);

        // Assert
        Assert.True(originalState[testPiece.Row, testPiece.Col] == copiedState[testPiece.Row, testPiece.Col]);
        Assert.True(originalState != copiedState);
    }

    #endregion
}