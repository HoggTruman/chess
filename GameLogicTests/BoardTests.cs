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

        Board testBoard = new();
        testBoard.AddPieces(pieces);

        // Assert
        testBoard.State[1, 1].Should().BeEquivalentTo(queen);
        testBoard.State[4, 5].Should().BeEquivalentTo(pawn);
    }

    #endregion

}