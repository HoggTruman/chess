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

    #endregion

}