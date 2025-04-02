using GameLogic;
using GameLogic.Enums;
using GameLogic.Moves;
using GameLogic.Pieces;
using FluentAssertions;

namespace GameLogicTests.Pieces;

public class PawnPieceTests
{
    #region GetTargetedSquares Tests

    [Fact]
    public void GetReachableSquares_WhitePawnNotMoved_ReturnsOneAndTwoAhead()
    {
        // Arrange
        Board board = new();

        var pawn = new PawnPiece(board, 6, 4, PieceColor.White);
        board.AddPiece(pawn);

        List<Square> expected = [
            new(5, 4),
            new(4, 4)
        ];

        // Act
        var result = pawn.GetReachableSquares();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }


    [Fact]
    public void GetReachableSquares_WhitePawnHasMoved_ReturnsOneAheadOnly()
    {
        // Arrange
        Board board = new();

        var pawn = new PawnPiece(board, 6, 4, PieceColor.White);
        board.AddPiece(pawn);

        StandardMove move = new(pawn.Square, new(5, 4));
        move.Apply(board);

        List<Square> expected = [
            new(4, 4)
        ];

        // Act
        var result = pawn.GetReachableSquares();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }


    [Fact]
    public void GetReachableSquares_WhitePawnCapturableEnemyPieces_ReturnsCapturableSquares()
    {
        // Arrange
        Board board = new();

        var pawn = new PawnPiece(board, 6, 4, PieceColor.White);
        var enemyPiece1 = new PawnPiece(board, 5, 3, PieceColor.Black);
        var enemyPiece2 = new PawnPiece(board, 5, 5, PieceColor.Black);
        board.AddPiece(pawn);
        board.AddPiece(enemyPiece1);
        board.AddPiece(enemyPiece2);

        List<Square> expected = [
            new(5, 4),
            new(4, 4),
            new(5, 3),
            new(5, 5)
        ];

        // Act
        var result = pawn.GetReachableSquares();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }


    [Fact]
    public void GetReachableSquares_WhitePawnWhenBlocked_ReturnsEmpty()
    {
        // Arrange
        Board board = new();

        var pawn = new PawnPiece(board, 6, 4, PieceColor.White);
        var blockingPiece = new PawnPiece(board, 5, 4, PieceColor.Black);
        board.AddPiece(pawn);
        board.AddPiece(blockingPiece);

        // Act
        var result = pawn.GetReachableSquares();

        // Assert
        result.Should().BeEmpty();
    }


    [Fact]
    public void GetTargetedSquares_WhitePawn_ReturnsTargetedSquares()
    {
        // Arrange
        Board board = new();

        var pawn = new PawnPiece(board, 4, 4, PieceColor.White);
        board.AddPiece(pawn);

        List<Square> expected = [
            new(3, 3),
            new(3, 5)
        ];

        // Act
        var result = pawn.GetTargetedSquares();

        // Assert 
        result.Should().BeEquivalentTo(expected);
    }


    [Fact]
    public void GetTargetedSquares_BlackPawn_ReturnsTargetedSquares()
    {
        // Arrange
        Board board = new();

        var pawn = new PawnPiece(board, 4, 4, PieceColor.Black);
        board.AddPiece(pawn);

        List<Square> expected = [
            new(5, 3),
            new(5, 5)
        ];

        // Act
        var result = pawn.GetTargetedSquares();

        // Assert 
        result.Should().BeEquivalentTo(expected);
    }

    #endregion



    #region GetReachableSquares Tests

    [Fact]
    public void GetReachableSquares_BlackPawnNotMoved_ReturnsOneAndTwoAhead()
    {
        // Arrange
        Board board = new();

        var pawn = new PawnPiece(board, 1, 4, PieceColor.Black);
        board.AddPiece(pawn);

        List<Square> expected = [
            new(2, 4),
            new(3, 4)
        ];

        // Act
        var result = pawn.GetReachableSquares();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }


    [Fact]
    public void GetReachableSquares_BlackPawnHasMoved_ReturnsOneAheadOnly()
    {
        // Arrange
        Board board = new();

        var pawn = new PawnPiece(board, 1, 4, PieceColor.Black);
        board.AddPiece(pawn);

        StandardMove move = new(pawn.Square, new(2, 4));
        move.Apply(board);

        List<Square> expected = [
            new(3, 4)
        ];

        // Act
        var result = pawn.GetReachableSquares();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }


    [Fact]
    public void GetReachableSquares_BlackPawnCapturableEnemyPieces_ReturnsCapturableSquares()
    {
        // Arrange
        Board board = new();

        var pawn = new PawnPiece(board, 1, 4, PieceColor.Black);
        var enemyPiece1 = new PawnPiece(board, 2, 3, PieceColor.White);
        var enemyPiece2 = new PawnPiece(board, 2, 5, PieceColor.White);
        board.AddPiece(pawn);
        board.AddPiece(enemyPiece1);
        board.AddPiece(enemyPiece2);

        List<Square> expected = [
            new(2, 4),
            new(3, 4),
            new(2, 3),
            new(2, 5)
        ];

        // Act
        var result = pawn.GetReachableSquares();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }


    [Fact]
    public void GetReachableSquares_BlackPawnWhenBlocked_ReturnsEmpty()
    {
        // Arrange
        Board board = new();

        var pawn = new PawnPiece(board, 1, 4, PieceColor.Black);
        var blockingPiece = new PawnPiece(board, 2, 4, PieceColor.White);
        board.AddPiece(pawn);
        board.AddPiece(blockingPiece);

        // Act
        var result = pawn.GetReachableSquares();

        // Assert
        result.Should().BeEmpty();
    }    

    #endregion



    #region GetEnPassantSquare Tests 

    [Fact]
    public void GetEnPassantSquare_WhenWhiteCanPerform_ReturnsSquare()
    {   
        // In this test, the black pawn starts on its starting square and advances two squares.
        // The white pawn starts on the correct row to perform En Passant.
        // The GetEnPassantSquare method should return the square where the white pawn lands after performing the move.

        // Arrange
        Board board = new();

        var blackPawn = new PawnPiece(board, 1, 3, PieceColor.Black);
        var whitePawn = new PawnPiece(board, 3, 4, PieceColor.White);
        board.AddPiece(blackPawn);
        board.AddPiece(whitePawn);

        StandardMove blackMove = new(blackPawn.Square, new(3, 3));

        Square expected = new(2, 3);


        // Act
        blackMove.Apply(board);
        var result = whitePawn.GetEnPassantSquare();

        // Assert 
        Assert.NotNull(result);
        result.Value.Should().Be(expected);
    }


    [Fact]
    public void GetEnPassantSquare_WhenBlackCanPerform_ReturnsSquare()
    {   
        // In this test, the white pawn starts on its starting square and advances two squares.
        // The black pawn starts on the correct row to perform En Passant.
        // The GetEnPassantSquare method should return the square where the black pawn lands after performing the move.

        // Arrange
        Board board = new();

        var blackPawn = new PawnPiece(board, 4, 3, PieceColor.Black);
        var whitePawn = new PawnPiece(board, 6, 4, PieceColor.White);
        board.AddPiece(blackPawn);
        board.AddPiece(whitePawn);

        StandardMove whiteMove = new(whitePawn.Square, new(4, 4));

        Square expected = new(5, 4);


        // Act
        whiteMove.Apply(board);
        var result = blackPawn.GetEnPassantSquare();

        // Assert 
        Assert.NotNull(result);
        result.Value.Should().Be(expected);
    }    


    [Fact]
    public void GetEnPassantSquare_WithEmptyMoveHistory_ReturnsNull()
    {   
        // In this test, the positioning is correct for black to perform en passant but there are no previous moves
        // The result should be null

        // Arrange
        Board board = new();

        var blackPawn = new PawnPiece(board, 4, 3, PieceColor.Black);
        var whitePawn = new PawnPiece(board, 4, 4, PieceColor.White);
        board.AddPiece(blackPawn);
        board.AddPiece(whitePawn);

        // Act
        var result = blackPawn.GetEnPassantSquare();

        // Assert 
        result.Should().BeNull();
    }


    [Fact]
    public void GetEnPassantSquare_WhenBlackAdvancesOneSquare_ReturnsNull()
    {   
        // In this test, the black pawn only advances one square to get in position
        // The result should be null

        // Arrange
        Board board = new();

        var blackPawn = new PawnPiece(board, 2, 3, PieceColor.Black);
        var whitePawn = new PawnPiece(board, 3, 4, PieceColor.White);
        board.AddPiece(blackPawn);
        board.AddPiece(whitePawn);

        StandardMove blackMove = new(blackPawn.Square, new(3, 3));

        // Act
        blackMove.Apply(board);
        var result = whitePawn.GetEnPassantSquare();

        // Assert 
        result.Should().BeNull();
    }


    [Fact]
    public void GetEnPassantSquare_WhenWhiteAdvancesOneSquare_ReturnsNull()
    {   
        // In this test, the white pawn only advances one square to get in position
        // The result should be null

        // Arrange
        Board board = new();

        var blackPawn = new PawnPiece(board, 4, 3, PieceColor.Black);
        var whitePawn = new PawnPiece(board, 5, 4, PieceColor.White);
        board.AddPiece(blackPawn);
        board.AddPiece(whitePawn);

        StandardMove whiteMove = new(whitePawn.Square, new(4, 4));

        // Act
        whiteMove.Apply(board);
        var result = blackPawn.GetEnPassantSquare();

        // Assert 
        result.Should().BeNull();
    }

    #endregion
}