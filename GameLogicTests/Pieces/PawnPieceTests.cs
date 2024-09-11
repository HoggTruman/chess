using FluentAssertions;
using GameLogic;
using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Interfaces;
using GameLogic.Moves;
using GameLogic.Pieces;

namespace GameLogicTests.Pieces;

public class PawnPieceTests
{
    #region GetTargetedSquares Tests

    [Fact]
    public void GetReachableSquares_WhitePawnNotMoved_ReturnsOneAndTwoAhead()
    {
        // Arrange
        Board board = new();

        var pawn = board.AddNewPiece<PawnPiece>(6, 4, PieceColor.White);

        List<(int row, int col)> expected = [
            (5, 4),
            (4, 4)
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

        var pawn = board.AddNewPiece<PawnPiece>(6, 4, PieceColor.White);

        StandardMove move = new(pawn.Square, (5, 4));
        board.StandardMove(move);

        List<(int row, int col)> expected = [
            (4, 4)
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

        var pawn = board.AddNewPiece<PawnPiece>(6, 4, PieceColor.White);
        var enemyPiece1 = board.AddNewPiece<PawnPiece>(5, 3, PieceColor.Black);
        var enemyPiece2 = board.AddNewPiece<PawnPiece>(5, 5, PieceColor.Black);

        List<(int row, int col)> expected = [
            (5, 4),
            (4, 4),
            (5, 3),
            (5, 5)
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

        var pawn = board.AddNewPiece<PawnPiece>(6, 4, PieceColor.White);
        var blockingPiece = board.AddNewPiece<PawnPiece>(5, 4, PieceColor.Black);

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

        var pawn = board.AddNewPiece<PawnPiece>(4, 4, PieceColor.White);

        List<(int row, int col)> expected = [
            (3, 3),
            (3, 5)
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

        var pawn = board.AddNewPiece<PawnPiece>(4, 4, PieceColor.Black);

        List<(int row, int col)> expected = [
            (5, 3),
            (5, 5)
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

        var pawn = board.AddNewPiece<PawnPiece>(1, 4, PieceColor.Black);

        List<(int row, int col)> expected = [
            (2, 4),
            (3, 4)
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

        var pawn = board.AddNewPiece<PawnPiece>(1, 4, PieceColor.Black);

        StandardMove move = new(pawn.Square, (2, 4));
        board.StandardMove(move);

        List<(int row, int col)> expected = [
            (3, 4)
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

        var pawn = board.AddNewPiece<PawnPiece>(1, 4, PieceColor.Black);
        var enemyPiece1 = board.AddNewPiece<PawnPiece>(2, 3, PieceColor.White);
        var enemyPiece2 = board.AddNewPiece<PawnPiece>(2, 5, PieceColor.White);

        List<(int row, int col)> expected = [
            (2, 4),
            (3, 4),
            (2, 3),
            (2, 5)
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

        var pawn = board.AddNewPiece<PawnPiece>(1, 4, PieceColor.Black);
        var blockingPiece = board.AddNewPiece<PawnPiece>(2, 4, PieceColor.White);

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

        var blackPawn = board.AddNewPiece<PawnPiece>(1, 3, PieceColor.Black);
        var whitePawn = board.AddNewPiece<PawnPiece>(3, 4, PieceColor.White);

        (int row, int col) blackTo = (3, 3);
        StandardMove blackMove = new(blackPawn.Square, blackTo);

        (int row, int col) expectedTo = (2, 3);
        (int row, int col) expectedCaptured = blackTo;


        // Act
        board.StandardMove(blackMove);
        var result = whitePawn.GetEnPassantSquares();

        // Assert 
        Assert.NotNull(result);
        result.Value.to.Should().Be(expectedTo);
        result.Value.captured.Should().Be(expectedCaptured);
    }


    [Fact]
    public void GetEnPassantSquare_WhenBlackCanPerform_ReturnsSquare()
    {   
        // In this test, the white pawn starts on its starting square and advances two squares.
        // The black pawn starts on the correct row to perform En Passant.
        // The GetEnPassantSquare method should return the square where the black pawn lands after performing the move.

        // Arrange
        Board board = new();

        var blackPawn = board.AddNewPiece<PawnPiece>(4, 3, PieceColor.Black);
        var whitePawn = board.AddNewPiece<PawnPiece>(6, 4, PieceColor.White);

        (int row, int col) whiteTo = (4, 4);
        StandardMove whiteMove = new(whitePawn.Square, whiteTo);

        (int row, int col) expectedTo = (5, 4);
        (int row, int col) expectedCaptured = whiteTo;


        // Act
        board.StandardMove(whiteMove);
        var result = blackPawn.GetEnPassantSquares();

        // Assert 
        Assert.NotNull(result);
        result.Value.to.Should().Be(expectedTo);
        result.Value.captured.Should().Be(expectedCaptured);
    }    


    [Fact]
    public void GetEnPassantSquare_WithEmptyMoveHistory_ReturnsNull()
    {   
        // In this test, the positioning is correct for black to perform en passant but there are no previous moves
        // The result should be null

        // Arrange
        Board board = new();

        var blackPawn = board.AddNewPiece<PawnPiece>(4, 3, PieceColor.Black);
        var whitePawn = board.AddNewPiece<PawnPiece>(4, 4, PieceColor.White);


        // Act
        var result = blackPawn.GetEnPassantSquares();

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

        var blackPawn = board.AddNewPiece<PawnPiece>(2, 3, PieceColor.Black);
        var whitePawn = board.AddNewPiece<PawnPiece>(3, 4, PieceColor.White);

        (int row, int col) blackTo = (3, 3);
        StandardMove blackMove = new(blackPawn.Square, blackTo);

        // Act
        board.StandardMove(blackMove);
        var result = whitePawn.GetEnPassantSquares();

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

        var blackPawn = board.AddNewPiece<PawnPiece>(4, 3, PieceColor.Black);
        var whitePawn = board.AddNewPiece<PawnPiece>(5, 4, PieceColor.White);

        (int row, int col) whiteTo = (4, 4);
        StandardMove whiteMove = new(whitePawn.Square, whiteTo);

        // Act
        board.StandardMove(whiteMove);
        var result = blackPawn.GetEnPassantSquares();

        // Assert 
        result.Should().BeNull();
    }

    #endregion
}