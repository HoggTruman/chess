using BetterGameLogic;
using BetterGameLogic.Enums;
using BetterGameLogic.Moves;
using BetterGameLogic.Pieces;
using FluentAssertions;

namespace BetterGameLogicTests.Moves;

public class CastleMoveTests
{
    #region Apply Tests

    [Fact]
    public void Apply_UpdatesBoardAndPieces()
    {
        // Arrange 
        Board board = new();

        Square kingFrom = new(0, 4);
        Square kingTo = new(0, 6);
        Square rookFrom = new(0, 7);
        Square rookTo = new(0, 5);

        var king = new KingPiece(board, kingFrom, PieceColor.White);
        var rook = new RookPiece(board, rookFrom, PieceColor.White);
        board.AddPiece(king);
        board.AddPiece(rook);

        CastleMove move = new(kingFrom, kingTo, rookFrom, rookTo);

        // Act
        move.Apply(board);

        // Assert
        king.Square.Should().Be(kingTo);
        rook.Square.Should().Be(rookTo);
        board.At(kingFrom).Should().BeNull();
        board.At(kingTo).Should().Be(king);
        board.At(rookFrom).Should().BeNull();
        board.At(rookTo).Should().Be(rook);
    }

    #endregion
}
