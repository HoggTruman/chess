using FluentAssertions;
using BetterGameLogic;
using BetterGameLogic.Constants;
using BetterGameLogic.Enums;
using BetterGameLogic.Moves;
using BetterGameLogic.Pieces;

namespace BetterGameLogicTests.Pieces;

public class RookPieceTests
{
    #region GetTargetedSquares Tests

    [Fact]
    public void GetTargetedSquares_ReturnsTargetedSquares()
    {
        // Arrange
        Board board = new();

        var rook = new RookPiece(board, 4, 4, PieceColor.White);
        var blockingPiece = new RookPiece(board, 4, 5, PieceColor.White);
        board.AddPiece(rook);
        board.AddPiece(blockingPiece);

        List<Square> expected = [
            new(4, 0),
            new(4, 1),
            new(4, 2),
            new(4, 3),
            new(4, 5),
            new(3, 4),
            new(2, 4),
            new(1, 4),
            new(0, 4),
            new(5, 4),
            new(6, 4),
            new(7, 4)            
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

        var rook = new RookPiece(board, 4, 4, PieceColor.White);
        var blockingPiece = new RookPiece(board, 4, 5, PieceColor.White);
        var nonBlockingPiece = new RookPiece(board, 7, 4, PieceColor.Black);
        board.AddPiece(rook);
        board.AddPiece(blockingPiece);
        board.AddPiece(nonBlockingPiece);

        List<Square> expected = [
            new(4, 0),
            new(4, 1),
            new(4, 2),
            new(4, 3),
            new(3, 4),
            new(2, 4),
            new(1, 4),
            new(0, 4),
            new(5, 4),
            new(6, 4),
            new(7, 4)            
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

        var king = new KingPiece(board, StartSquares.WhiteKing, PieceColor.White);
        var rook = new RookPiece(board, StartSquares.WhiteRookK, PieceColor.White);
        board.AddPiece(king);
        board.AddPiece(rook);

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

        var king = new KingPiece(board, StartSquares.WhiteKing, PieceColor.White);
        var rook = new RookPiece(board, StartSquares.WhiteRookK, PieceColor.White);
        var blockingPiece = new QueenPiece(board, StartSquares.WhiteKing.Row, blockedCol, PieceColor.White);
        board.AddPiece(king);
        board.AddPiece(rook);
        board.AddPiece(blockingPiece);        

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

        var king = new KingPiece(board, StartSquares.WhiteKing, PieceColor.White);
        var rook = new RookPiece(board, StartSquares.WhiteRookK, PieceColor.White);
        var enemyRook = new RookPiece(board, StartSquares.BlackKing.Row, targetedCol, PieceColor.Black);
        board.AddPiece(king);
        board.AddPiece(rook);
        board.AddPiece(enemyRook);        

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

        var king = new KingPiece(board, StartSquares.WhiteKing, PieceColor.White);
        var rook = new RookPiece(board, StartSquares.WhiteRookK, PieceColor.White);
        var enemyRook = new RookPiece(board, StartSquares.BlackKing.Row, StartSquares.WhiteRookK.Col, PieceColor.Black);
        board.AddPiece(king);
        board.AddPiece(rook);
        board.AddPiece(enemyRook);

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

        var king = new KingPiece(board, StartSquares.WhiteKing.Row - 1, StartSquares.WhiteKing.Col, PieceColor.White);
        var rook = new RookPiece(board, StartSquares.WhiteRookK, PieceColor.White);
        board.AddPiece(king);
        board.AddPiece(rook);

        StandardMove kingMove = new(king.Square, StartSquares.WhiteKing);
        kingMove.Apply(board);

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

        var king = new KingPiece(board, StartSquares.WhiteKing, PieceColor.White);
        var rook = new RookPiece(board, StartSquares.WhiteRookK.Row - 1, StartSquares.WhiteRookK.Col, PieceColor.White);
        board.AddPiece(king);
        board.AddPiece(rook);

        StandardMove rookMove = new(rook.Square, StartSquares.WhiteRookK);
        rookMove.Apply(board);

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

        var king = new KingPiece(board, StartSquares.WhiteKing, PieceColor.White);
        var rook = new RookPiece(board, StartSquares.BlackRookK, PieceColor.White);
        board.AddPiece(king);
        board.AddPiece(rook);

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

        var king = new KingPiece(board, StartSquares.WhiteKing, PieceColor.White);
        var rook = new RookPiece(board, StartSquares.BlackKing, PieceColor.White);
        board.AddPiece(king);
        board.AddPiece(rook);

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

        var king = new KingPiece(board, StartSquares.WhiteKing, PieceColor.White);
        var rook = new RookPiece(board, StartSquares.WhiteKnightK, PieceColor.White);
        board.AddPiece(king);
        board.AddPiece(rook);

        // Act
        var result = rook.CanCastle();

        // Assert 
        result.Should().BeFalse();
    }

    #endregion
}