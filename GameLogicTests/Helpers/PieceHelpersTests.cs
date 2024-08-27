using FluentAssertions;
using GameLogic;
using GameLogic.Helpers;
using GameLogic.Pieces;

namespace GameLogicTests.Helpers;

public class PieceHelpersTests
{
    #region ScanRowAndCol Tests

    [Fact]
	public void ScanRowAndCol_WithEmptyBoard_ReturnsAllSquares()
	{
		// Arrange
		int testRow = 3;
		int testCol = 3;
		Board testBoard = new();
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
		var result = PieceHelpers.ScanRowAndCol(testRow, testCol, testBoard);

		// Assert
		result.Should().BeEquivalentTo(expected);
	}


	[Fact]
	public void ScanRowAndCol_WithBlockingPieces_ReturnsTargetableSquares()
	{
		// Arrange
		int testRow = 4;
		int testCol = 4;
		Board testBoard = new();

		var testQueen1 = new QueenPiece(testBoard, testRow, 2);
		var testQueen2 = new QueenPiece(testBoard, testRow, 6);
		var testQueen3 = new QueenPiece(testBoard, 2, testCol);
		var testQueen4 = new QueenPiece(testBoard, 6, testCol);

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
		var result = PieceHelpers.ScanRowAndCol(testRow, testCol, testBoard);

		// Assert
		result.Should().BeEquivalentTo(expected);
	}


	[Fact]
	public void ScanRowAndCol_WithPieceInMinCorner_ReturnsInBoundsSquares()
	{
		// Arrange
		int testRow = 0;
		int testCol = 0;
		Board testBoard = new();

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
		var result = PieceHelpers.ScanRowAndCol(testRow, testCol, testBoard);

		// Assert
		result.Should().BeEquivalentTo(expected);
	}


	[Fact]
	public void ScanRowAndCol_WithPieceInMaxCorner_ReturnsInBoundsSquares()
	{
		// Arrange
		int testRow = 7;
		int testCol = 7;
		Board testBoard = new();

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
		var result = PieceHelpers.ScanRowAndCol(testRow, testCol, testBoard);

		// Assert
		result.Should().BeEquivalentTo(expected);
	}


	[Fact]
	public void ScanRowAndCol_WhenSurrounded_ReturnsSurroundingSquares()
	{
		// Arrange
		int testRow = 3;
		int testCol = 4;
		Board testBoard = new();

		var testQueen1 = new QueenPiece(testBoard, testRow, testCol - 1);
		var testQueen2 = new QueenPiece(testBoard, testRow, testCol + 1);
		var testQueen3 = new QueenPiece(testBoard, testRow - 1, testCol);
		var testQueen4 = new QueenPiece(testBoard, testRow + 1, testCol);

		List<(int row, int col)> expected = [
			new(testRow, testCol - 1),
			new(testRow, testCol + 1),
			new(testRow - 1, testCol),
			new(testRow + 1, testCol),
		];

		// Act
		var result = PieceHelpers.ScanRowAndCol(testRow, testCol, testBoard);

		// Assert
		result.Should().BeEquivalentTo(expected);
	}

	#endregion

	#region ScanDiagonals Tests

	[Fact]
	public void ScanDiagonals_withEmptyBoard_ReturnsAllSquares()
	{
		// Arrange
		int testRow = 3;
		int testCol = 3;
		Board testBoard = new();

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
		var result = PieceHelpers.ScanDiagonals(testRow, testCol, testBoard);

		// Assert
		result.Should().BeEquivalentTo(expected);
	}


	[Fact]
	public void ScanDiagonals_InCorner_ReturnsInBoundSquares()
	{
		// Arrange
		int testRow = 0;
		int testCol = 0;
		Board testBoard = new();

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
		var result = PieceHelpers.ScanDiagonals(testRow, testCol, testBoard);

		// Assert
		result.Should().BeEquivalentTo(expected);
	}


	[Fact]
	public void ScanDiagonals_WhenSurrounded_ReturnsSurroundingSquares()
	{
		// Arrange
		int testRow = 3;
		int testCol = 4;
		Board testBoard = new();

		var testQueen1 = new QueenPiece(testBoard, testRow - 1, testCol - 1);
		var testQueen2 = new QueenPiece(testBoard, testRow + 1, testCol + 1);
		var testQueen3 = new QueenPiece(testBoard, testRow - 1, testCol + 1);
		var testQueen4 = new QueenPiece(testBoard, testRow + 1, testCol - 1);

		List<(int row, int col)> expected = [
			new(testRow - 1, testCol - 1),
			new(testRow + 1, testCol + 1),
			new(testRow - 1, testCol + 1),
			new(testRow + 1, testCol - 1),
		];

		// Act
		var result = PieceHelpers.ScanDiagonals(testRow, testCol, testBoard);

		// Assert
		result.Should().BeEquivalentTo(expected);
	}

    #endregion

}