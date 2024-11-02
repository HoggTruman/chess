using FluentAssertions;
using GameLogic;
using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Pieces;

namespace GameLogicTests.Helpers;

public class PieceHelpersTests
{
    #region GetTargetedRowColSquares Tests

    [Fact]
	public void GetTargetedRowColSquares_WithEmptyBoard_ReturnsAllSquares()
	{
		// Arrange
        Board board = new();
		int testRow = 3;
		int testCol = 3;

		List<(int row, int col)> expected = [
			new(testRow, 0),
			new(testRow, 1),
			new(testRow, 2),
			new(testRow, 4),
			new(testRow, 5),
			new(testRow, 6),
			new(testCol, 7),
			new(0, testCol),
			new(1, testCol),
			new(2, testCol),
			new(4, testCol),
			new(5, testCol),
			new(6, testCol),
			new(7, testCol),
		];

		// Act
		var result = PieceHelpers.GetTargetedRowColSquares(testRow, testCol, board);

		// Assert
		result.Should().BeEquivalentTo(expected);
	}


	[Fact]
	public void GetTargetedRowColSquares_WithBlockingPieces_ReturnsTargetableSquares()
	{
		// Arrange
        Board board = new();
		int testRow = 4;
		int testCol = 4;

		var queen1 = board.AddNewPiece<QueenPiece>(testRow, 2, PieceColor.White);
		var queen2 = board.AddNewPiece<QueenPiece>(testRow, 6, PieceColor.White);
		var queen3 = board.AddNewPiece<QueenPiece>(2, testCol, PieceColor.White);
		var queen4 = board.AddNewPiece<QueenPiece>(6, testCol, PieceColor.White);

		List<(int row, int col)> expected = [
			new(testRow, 3),
			new(testRow, 2),
			new(testRow, 5),
			new(testRow, 6),
			new(3, testCol),
			new(2, testCol),
			new(5, testCol),
			new(6, testCol),
		];

		// Act 
		var result = PieceHelpers.GetTargetedRowColSquares(testRow, testCol, board);

		// Assert
		result.Should().BeEquivalentTo(expected);
	}


	[Fact]
	public void GetTargetedRowColSquares_InMinCorner_ReturnsInBoundsSquares()
	{
		// Arrange
        Board board = new();
		int testRow = 0;
		int testCol = 0;

		List<(int row, int col)> expected = [
			new(testRow, 1),
			new(testRow, 2),
			new(testRow, 3),
			new(testRow, 4),
			new(testRow, 5),
			new(testRow, 6),
			new(testCol, 7),
			new(1, testCol),
			new(2, testCol),
			new(3, testCol),
			new(4, testCol),
			new(5, testCol),
			new(6, testCol),
			new(7, testCol),
		];

		// Act 
		var result = PieceHelpers.GetTargetedRowColSquares(testRow, testCol, board);

		// Assert
		result.Should().BeEquivalentTo(expected);
	}


	[Fact]
	public void GetTargetedRowColSquares_InMaxCorner_ReturnsInBoundsSquares()
	{
		// Arrange
        Board board = new();
		int testRow = 7;
		int testCol = 7;

		List<(int row, int col)> expected = [
			new(testRow, 0),
			new(testRow, 1),
			new(testRow, 2),
			new(testRow, 3),
			new(testRow, 4),
			new(testRow, 5),
			new(testCol, 6),
			new(0, testCol),
			new(1, testCol),
			new(2, testCol),
			new(3, testCol),
			new(4, testCol),
			new(5, testCol),
			new(6, testCol),
		];

		// Act 
		var result = PieceHelpers.GetTargetedRowColSquares(testRow, testCol, board);

		// Assert
		result.Should().BeEquivalentTo(expected);
	}


	[Fact]
	public void GetTargetedRowColSquares_WhenSurrounded_ReturnsSurroundingSquares()
	{
		// Arrange
        Board board = new();
		int testRow = 3;
		int testCol = 4;
		
		var queen1 = board.AddNewPiece<QueenPiece>(testRow, testCol - 1, PieceColor.White);
		var queen2 = board.AddNewPiece<QueenPiece>(testRow, testCol + 1, PieceColor.White);
		var queen3 = board.AddNewPiece<QueenPiece>(testRow - 1, testCol, PieceColor.White);
		var queen4 = board.AddNewPiece<QueenPiece>(testRow + 1, testCol, PieceColor.White);

		List<(int row, int col)> expected = [
			new(testRow, testCol - 1),
			new(testRow, testCol + 1),
			new(testRow - 1, testCol),
			new(testRow + 1, testCol),
		];

		// Act
		var result = PieceHelpers.GetTargetedRowColSquares(testRow, testCol, board);

		// Assert
		result.Should().BeEquivalentTo(expected);
	}

	#endregion



	#region GetTargetedDiagonalSquares Tests

	[Fact]
	public void GetTargetedDiagonalSquares_withEmptyBoard_ReturnsAllSquares()
	{
		// Arrange
        Board board = new();
		int testRow = 3;
		int testCol = 3;
		
		List<(int row, int col)> expected = [
			new(0, 0),
			new(1, 1),
			new(2, 2),
			new(4, 4),
			new(5, 5),
			new(6, 6),
			new(7, 7),
			new(2, 4),
			new(1, 5),
			new(0, 6),
			new(4, 2),
			new(5, 1),
			new(6, 0)
		];

		// Act
		var result = PieceHelpers.GetTargetedDiagonalSquares(testRow, testCol, board);

		// Assert
		result.Should().BeEquivalentTo(expected);
	}


	[Fact]
	public void GetTargetedDiagonalSquares_InCorner_ReturnsInBoundSquares()
	{
		// Arrange
        Board board = new();
		int testRow = 0;
		int testCol = 0;

		List<(int row, int col)> expected = [
			new(1, 1),
			new(2, 2),
			new(3, 3),
			new(4, 4),
			new(5, 5),
			new(6, 6),
			new(7, 7),
		];

		// Act
		var result = PieceHelpers.GetTargetedDiagonalSquares(testRow, testCol, board);

		// Assert
		result.Should().BeEquivalentTo(expected);
	}


	[Fact]
	public void GetTargetedDiagonalSquares_WhenSurrounded_ReturnsSurroundingSquares()
	{
		// Arrange
        Board board = new();
		int testRow = 3;
		int testCol = 4;

		var queen1 = board.AddNewPiece<QueenPiece>(testRow - 1, testCol - 1, PieceColor.White);
		var queen2 = board.AddNewPiece<QueenPiece>(testRow + 1, testCol + 1, PieceColor.White);
		var queen3 = board.AddNewPiece<QueenPiece>(testRow - 1, testCol + 1, PieceColor.White);
		var queen4 = board.AddNewPiece<QueenPiece>(testRow + 1, testCol - 1, PieceColor.White);

		List<(int row, int col)> expected = [
			new(testRow - 1, testCol - 1),
			new(testRow + 1, testCol + 1),
			new(testRow - 1, testCol + 1),
			new(testRow + 1, testCol - 1),
		];

		// Act
		var result = PieceHelpers.GetTargetedDiagonalSquares(testRow, testCol, board);

		// Assert
		result.Should().BeEquivalentTo(expected);
	}

    #endregion

}