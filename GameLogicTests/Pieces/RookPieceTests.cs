using FluentAssertions;
using GameLogic;
using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Interfaces;
using GameLogic.Moves;
using GameLogic.Pieces;

namespace GameLogicTests.Pieces;

public class RookPieceTests
{
    #region CanCastle Tests 

    [Fact]
    public void CanCastle_WhiteKingSideRook_ReturnsTrue()
    {
        // Arrange
        Board testBoard = new();

        KingPiece king = new(StartSquares.WhiteKing.row, StartSquares.WhiteKing.col);
        RookPiece rook = new(StartSquares.WhiteRookK.row, StartSquares.WhiteRookK.col);

        List<IPiece> pieces = [king, rook];

        testBoard.AddPieces(pieces);

        // Act
        var result = rook.CanCastle(testBoard);

        // Assert 
        result.Should().BeTrue();
    }


    [Theory]
    [InlineData(5)] // bishop col
    [InlineData(6)] // knight col
    public void CanCastle_WhiteKingSideWhenBlocked_ReturnsFalse(int blockedCol)
    {
        // Arrange
        Board testBoard = new();

        KingPiece king = new(StartSquares.WhiteKing.row, StartSquares.WhiteKing.col);
        RookPiece rook = new(StartSquares.WhiteRookK.row, StartSquares.WhiteRookK.col);

        QueenPiece blockingPiece = new(StartSquares.WhiteKing.row, blockedCol);

        List<IPiece> pieces = [king, rook, blockingPiece];

        testBoard.AddPieces(pieces);

        // Act
        var result = rook.CanCastle(testBoard);

        // Assert 
        result.Should().BeFalse();
    }


    [Theory]
    [InlineData(4)] // king col
    [InlineData(5)] // bishop col
    [InlineData(6)] // knight col
    public void CanCastle_WhiteKingSideWhenKingTargeted_ReturnsFalse(int targetedCol)
    {
        // Tests that CanCastle returns false when each of the squares the king passes
        // through (including its starting square) are targeted by an enemy piece

        // Arrange
        Board testBoard = new();

        KingPiece king = new(StartSquares.WhiteKing.row, StartSquares.WhiteKing.col, Color.White);
        RookPiece rook = new(StartSquares.WhiteRookK.row, StartSquares.WhiteRookK.col, Color.White);

        RookPiece enemyPiece = new(StartSquares.BlackKing.row, targetedCol, Color.Black);

        List<IPiece> pieces = [king, rook, enemyPiece];

        testBoard.AddPieces(pieces);

        // Act
        var result = rook.CanCastle(testBoard);

        // Assert 
        result.Should().BeFalse();
    }

    [Fact]
    public void CanCastle_WhiteKingSideWhenOnlyRookTargeted_ReturnsTrue()
    {
        // Tests that CanCastle returns true when each of the squares that only the rook
        // passes through are targeted by an enemy piece

        // Arrange
        Board testBoard = new();

        KingPiece king = new(StartSquares.WhiteKing.row, StartSquares.WhiteKing.col, Color.White);
        RookPiece rook = new(StartSquares.WhiteRookK.row, StartSquares.WhiteRookK.col, Color.White);

        RookPiece enemyPiece = new(StartSquares.BlackKing.row, StartSquares.WhiteRookK.col, Color.Black);

        List<IPiece> pieces = [king, rook, enemyPiece];

        testBoard.AddPieces(pieces);

        // Act
        var result = rook.CanCastle(testBoard);

        // Assert 
        result.Should().BeTrue();
    }


    [Fact]
    public void CanCastle_WhiteKingSideWhenKingHasMoved_ReturnsFalse()
    {
        // Arrange
        Board testBoard = new();

        KingPiece king = new(StartSquares.WhiteKing.row + 1, StartSquares.WhiteKing.col);
        RookPiece rook = new(StartSquares.WhiteRookK.row, StartSquares.WhiteRookK.col);

        List<IPiece> pieces = [king, rook];

        testBoard.AddPieces(pieces);

        Move kingMove = new(MoveType.Move, king.Square, StartSquares.WhiteKing);
        testBoard.HandleMove(kingMove);

        // Act
        var result = rook.CanCastle(testBoard);

        // Assert 
        result.Should().BeFalse();
    }


    [Fact]
    public void CanCastle_WhiteKingSideWhenRookHasMoved_ReturnsFalse()
    {
        // Arrange
        Board testBoard = new();

        KingPiece king = new(StartSquares.WhiteKing.row, StartSquares.WhiteKing.col);
        RookPiece rook = new(StartSquares.WhiteRookK.row + 1, StartSquares.WhiteRookK.col);

        List<IPiece> pieces = [king, rook];

        testBoard.AddPieces(pieces);

        Move rookMove = new(MoveType.Move, rook.Square, StartSquares.WhiteRookK);
        testBoard.HandleMove(rookMove);

        // Act
        var result = rook.CanCastle(testBoard);

        // Assert 
        result.Should().BeFalse();
    }

    #endregion
}