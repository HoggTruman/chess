using FluentAssertions;
using GameLogic;
using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Moves;
using GameLogic.Pieces;

namespace GameLogicTests.Pieces;

public class RookPieceTests
{
    #region GetTargetedSquares Tests

    [Fact]
    public void GetTargetedSquares_ReturnsTargetedSquares()
    {
        // Arrange
        Board board = new();

        var rook = board.AddNewPiece<RookPiece>(4, 4, PieceColor.White);

        var blockingPiece = board.AddNewPiece<RookPiece>(4, 5, PieceColor.White);

        List<(int row, int col)> expected = [
            (4, 0),
            (4, 1),
            (4, 2),
            (4, 3),
            (4, 5),
            (3, 4),
            (2, 4),
            (1, 4),
            (0, 4),
            (5, 4),
            (6, 4),
            (7, 4)            
        ];

        // Act
        var result = rook.GetTargetedSquares();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    #endregion



    #region GetReachableSquares Tests

    [Fact]
    public void GetReachableSquares_ReturnsReachableSquares()
    {
        // Arrange
        Board board = new();

        var rook = board.AddNewPiece<RookPiece>(4, 4, PieceColor.White);

        var blockingPiece = board.AddNewPiece<RookPiece>(4, 5, PieceColor.White);
        var nonblockingPiece = board.AddNewPiece<RookPiece>(7, 4, PieceColor.Black);

        List<(int row, int col)> expected = [
            (4, 0),
            (4, 1),
            (4, 2),
            (4, 3),
            (3, 4),
            (2, 4),
            (1, 4),
            (0, 4),
            (5, 4),
            (6, 4),
            (7, 4)            
        ];

        // Act
        var result = rook.GetReachableSquares();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    #endregion



    #region CanCastle Tests 

    [Fact]
    public void CanCastle_WhiteKingSideRook_ReturnsTrue()
    {
        // Arrange
        Board board = new();

        var king = board.AddNewPiece<KingPiece>(StartSquares.WhiteKing);
        var rook = board.AddNewPiece<RookPiece>(StartSquares.WhiteRookK);

        // Act
        var result = rook.CanCastle();

        // Assert 
        result.Should().BeTrue();
    }


    [Theory]
    [InlineData(5)] // bishop col
    [InlineData(6)] // knight col
    public void CanCastle_WhiteKingSideWhenBlocked_ReturnsFalse(int blockedCol)
    {
        // Arrange
        Board board = new();

        var king = board.AddNewPiece<KingPiece>(StartSquares.WhiteKing);
        var rook = board.AddNewPiece<RookPiece>(StartSquares.WhiteRookK);

        var blockingPiece = board.AddNewPiece<QueenPiece>(StartSquares.WhiteKing.row, blockedCol);

        // Act
        var result = rook.CanCastle();

        // Assert 
        result.Should().BeFalse();
    }


    [Theory]
    [InlineData(4)] // king col
    [InlineData(5)] // bishop col
    [InlineData(6)] // knight col
    public void CanCastle_WhiteKingSideWhenKingTargeted_ReturnsFalse(int targetedCol)
    {
        // Tests that CanCastle returns false when any of the squares the king passes
        // through (including its starting square) are targeted by an enemy piece

        // Arrange
        Board board = new();

        var king = board.AddNewPiece<KingPiece>(StartSquares.WhiteKing, PieceColor.White);
        var rook = board.AddNewPiece<RookPiece>(StartSquares.WhiteRookK, PieceColor.White);

        var enemyRook = board.AddNewPiece<RookPiece>(StartSquares.BlackKing.row, targetedCol, PieceColor.Black);

        // Act
        var result = rook.CanCastle();

        // Assert 
        result.Should().BeFalse();
    }


    [Fact]
    public void CanCastle_WhiteKingSideWhenOnlyRookTargeted_ReturnsTrue()
    {
        // Tests that CanCastle returns true when any of the squares that only the rook
        // passes through are targeted by an enemy piece

        // Arrange
        Board board = new();

        var king = board.AddNewPiece<KingPiece>(StartSquares.WhiteKing, PieceColor.White);
        var rook = board.AddNewPiece<RookPiece>(StartSquares.WhiteRookK, PieceColor.White);

        var enemyRook = board.AddNewPiece<RookPiece>(StartSquares.BlackKing.row, StartSquares.WhiteRookK.col, PieceColor.Black);

        // Act
        var result = rook.CanCastle();

        // Assert 
        result.Should().BeTrue();
    }


    [Fact]
    public void CanCastle_WhiteKingSideWhenKingHasMoved_ReturnsFalse()
    {
        // Arrange
        Board board = new();

        var king = board.AddNewPiece<KingPiece>(StartSquares.WhiteKing.row - 1, StartSquares.WhiteKing.col);
        var rook = board.AddNewPiece<RookPiece>(StartSquares.WhiteRookK);

        StandardMove kingMove = new(king.Square, StartSquares.WhiteKing);
        board.HandleMove(kingMove);

        // Act
        var result = rook.CanCastle();

        // Assert 
        result.Should().BeFalse();
    }


    [Fact]
    public void CanCastle_WhiteKingSideWhenRookHasMoved_ReturnsFalse()
    {
        // Arrange
        Board board = new();

        var king = board.AddNewPiece<KingPiece>(StartSquares.WhiteKing);
        var rook = board.AddNewPiece<RookPiece>(StartSquares.WhiteRookK.row - 1, StartSquares.WhiteRookK.col);

        StandardMove rookMove = new(rook.Square, StartSquares.WhiteRookK);
        board.HandleMove(rookMove);

        // Act
        var result = rook.CanCastle();

        // Assert 
        result.Should().BeFalse();
    }


    [Fact]
    public void CanCastle_WhenCreatedOnEnemyRookSquare_ReturnsFalse()
    {
        // This tests that CanCastle returns false when a rook is created 
        // in a corner on the enemy side. This is to test pawn promoted 
        // rook pieces  

        // Arrange
        Board board = new();

        var king = board.AddNewPiece<KingPiece>(StartSquares.WhiteKing, PieceColor.White);
        var rook = board.AddNewPiece<RookPiece>(StartSquares.BlackRookK, PieceColor.White);

        // Act
        var result = rook.CanCastle();

        // Assert 
        result.Should().BeFalse();
    }


    [Fact]
    public void CanCastle_WhenCreatedOnEnemyKingSquare_ReturnsFalse()
    {
        // This tests that CanCastle returns false when a rook is created 
        // on the enemy king's starting square. This is to test pawn promoted 
        // rook pieces  

        // Arrange
        Board board = new();

        var king = board.AddNewPiece<KingPiece>(StartSquares.WhiteKing, PieceColor.White);
        var rook = board.AddNewPiece<RookPiece>(StartSquares.BlackKing, PieceColor.White);

        // Act
        var result = rook.CanCastle();

        // Assert 
        result.Should().BeFalse();
    }


    [Fact]
    public void CanCastle_WhenRookStartsCorrectRowWrongCol_ReturnsFalse()
    {
        // This tests that CanCastle returns false when a rook is created 
        // on a square on the king's row but not in a corner. This can never
        // occur during an actual game but is included just to be thorough

        // Arrange
        Board board = new();

        var king = board.AddNewPiece<KingPiece>(StartSquares.WhiteKing, PieceColor.White);
        var rook = board.AddNewPiece<RookPiece>(StartSquares.WhiteKnightK, PieceColor.White);

        // Act
        var result = rook.CanCastle();

        // Assert 
        result.Should().BeFalse();
    }

    #endregion
}