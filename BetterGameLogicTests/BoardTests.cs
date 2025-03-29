using BetterGameLogic;
using BetterGameLogic.Constants;
using BetterGameLogic.Enums;
using BetterGameLogic.Pieces;
using FluentAssertions;

namespace BetterGameLogicTests;

public class BoardTests
{
    #region On Construction Tests

    [Fact]
    public void BoardConstructs()
    {
        // Arrange 
        Board testBoard = new();
        IPiece?[,] emptyBoardState = new IPiece?[Board.BoardSize, Board.BoardSize];

        // Assert
        testBoard.State.Should().BeEquivalentTo(emptyBoardState);
        testBoard.Pieces[PieceColor.White].Should().BeEmpty();
        testBoard.Pieces[PieceColor.Black].Should().BeEmpty();
    }

    #endregion



    #region Initialize Tests

    [Fact]
    public void Initialize_AddsPiecesInCorrectPositions()
    {
        // Arrange
        Board board = new();
        
        // Act
        board.Initialize();

        // Assert
        Assert.True(board.At(StartSquares.WhiteRookQ) is RookPiece WRQ && WRQ.Color == PieceColor.White);
        Assert.True(board.At(StartSquares.WhiteKnightQ) is KnightPiece WKQ && WKQ.Color == PieceColor.White);
        Assert.True(board.At(StartSquares.WhiteBishopQ) is BishopPiece WBQ && WBQ.Color == PieceColor.White);
        Assert.True(board.At(StartSquares.WhiteQueen) is QueenPiece WQ && WQ.Color == PieceColor.White);
        Assert.True(board.At(StartSquares.WhiteKing) is KingPiece WK && WK.Color == PieceColor.White);
        Assert.True(board.At(StartSquares.WhiteBishopK) is BishopPiece WBK && WBK.Color == PieceColor.White);
        Assert.True(board.At(StartSquares.WhiteKnightK) is KnightPiece WKK && WKK.Color == PieceColor.White);
        Assert.True(board.At(StartSquares.WhiteRookK) is RookPiece WRK && WRK.Color == PieceColor.White);

        Assert.True(board.At(StartSquares.WhitePawnA) is PawnPiece WPA && WPA.Color == PieceColor.White);
        Assert.True(board.At(StartSquares.WhitePawnB) is PawnPiece WPB && WPB.Color == PieceColor.White);
        Assert.True(board.At(StartSquares.WhitePawnC) is PawnPiece WPC && WPC.Color == PieceColor.White);
        Assert.True(board.At(StartSquares.WhitePawnD) is PawnPiece WPD && WPD.Color == PieceColor.White);
        Assert.True(board.At(StartSquares.WhitePawnE) is PawnPiece WPE && WPE.Color == PieceColor.White);
        Assert.True(board.At(StartSquares.WhitePawnF) is PawnPiece WPF && WPF.Color == PieceColor.White);
        Assert.True(board.At(StartSquares.WhitePawnG) is PawnPiece WPG && WPG.Color == PieceColor.White);
        Assert.True(board.At(StartSquares.WhitePawnH) is PawnPiece WPH && WPH.Color == PieceColor.White);

        Assert.True(board.At(StartSquares.BlackRookQ) is RookPiece BRQ && BRQ.Color == PieceColor.Black);
        Assert.True(board.At(StartSquares.BlackKnightQ) is KnightPiece BKQ && BKQ.Color == PieceColor.Black);
        Assert.True(board.At(StartSquares.BlackBishopQ) is BishopPiece BBQ && BBQ.Color == PieceColor.Black);
        Assert.True(board.At(StartSquares.BlackQueen) is QueenPiece BQ && BQ.Color == PieceColor.Black);
        Assert.True(board.At(StartSquares.BlackKing) is KingPiece BK && BK.Color == PieceColor.Black);
        Assert.True(board.At(StartSquares.BlackBishopK) is BishopPiece BBK && BBK.Color == PieceColor.Black);
        Assert.True(board.At(StartSquares.BlackKnightK) is KnightPiece BKK && BKK.Color == PieceColor.Black);
        Assert.True(board.At(StartSquares.BlackRookK) is RookPiece BRK && BRK.Color == PieceColor.Black);

        Assert.True(board.At(StartSquares.BlackPawnA) is PawnPiece BPA && BPA.Color == PieceColor.Black);
        Assert.True(board.At(StartSquares.BlackPawnB) is PawnPiece BPB && BPB.Color == PieceColor.Black);
        Assert.True(board.At(StartSquares.BlackPawnC) is PawnPiece BPC && BPC.Color == PieceColor.Black);
        Assert.True(board.At(StartSquares.BlackPawnD) is PawnPiece BPD && BPD.Color == PieceColor.Black);
        Assert.True(board.At(StartSquares.BlackPawnE) is PawnPiece BPE && BPE.Color == PieceColor.Black);
        Assert.True(board.At(StartSquares.BlackPawnF) is PawnPiece BPF && BPF.Color == PieceColor.Black);
        Assert.True(board.At(StartSquares.BlackPawnG) is PawnPiece BPG && BPG.Color == PieceColor.Black);
        Assert.True(board.At(StartSquares.BlackPawnH) is PawnPiece BPH && BPH.Color == PieceColor.Black);
    }

    #endregion



    #region AddPieceTests

    [Fact]
    public void AddPiece_ValidInput()
    {
        // Arrange
        Board board = new();
        int row = 4;
        int col = 5;
        PieceColor color = PieceColor.White;

        // Act
        board.AddPiece(new QueenPiece(board, row, col, color));
        var resultPiece = board.State[row, col];

        // Assert
        Assert.NotNull(resultPiece);
        resultPiece.Row.Should().Be(row);
        resultPiece.Col.Should().Be(col);
        resultPiece.Color.Should().Be(color);
    }


    [Fact]
    public void AddPiece_TwoKingPiecesOfSameColor_ThrowsArgumentException()
    {
        // Arrange
        Board board = new();
        board.AddPiece(new KingPiece(board, 0, 0, PieceColor.White));

        // Act + Assert
        Assert.Throws<ArgumentException>(() => board.AddPiece(new KingPiece(board, 1, 1, PieceColor.White)));
    }


    [Fact]
    public void AddPiece_ToOccupiedSquare_ThrowsArgumentException()
    {
        // Arrange
        Board board = new();
        board.AddPiece(new QueenPiece(board, 0, 0, PieceColor.White));

        // Act + Assert
        Assert.Throws<ArgumentException>(() => board.AddPiece(new QueenPiece(board, 0, 0, PieceColor.Black)));
    }


    [Fact]
    public void AddPiece_RowIndexOutOfBounds_ThrowsArgumentException()
    {
        // Arrange
        Board board = new();

        // Act + Assert
        Assert.Throws<IndexOutOfRangeException>(() => board.AddPiece(new QueenPiece(board, -1, 0, PieceColor.Black)));
        Assert.Throws<IndexOutOfRangeException>(() => board.AddPiece(new QueenPiece(board, 8, 0, PieceColor.Black)));
    }


    [Fact]
    public void AddNewPiece_ColIndexOutOfBounds_ThrowsArgumentException()
    {
        // Arrange
        Board board = new();

        // Act + Assert
        Assert.Throws<IndexOutOfRangeException>(() => board.AddPiece(new QueenPiece(board, 0, -1, PieceColor.Black)));
        Assert.Throws<IndexOutOfRangeException>(() => board.AddPiece(new QueenPiece(board, 0, 8, PieceColor.Black)));
    }

    #endregion
    


    #region IsInBounds Tests

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 7)]
    [InlineData(7, 0)]
    [InlineData(7, 7)]
    [InlineData(0, 4)]
    [InlineData(4, 0)]
    [InlineData(7, 4)]
    [InlineData(4, 7)]
    [InlineData(1, 2)]
    [InlineData(5, 6)]
    [InlineData(4, 4)]
    public void IsInBounds_WithInBoundsSquare_ReturnsTrue(int row, int col)
    {
        // Arrange
        Square square = new(row, col);

        // Act
        var result = Board.IsInBounds(square);

        // Assert
        result.Should().BeTrue();
    }


    [Theory]
    [InlineData(-1, -1)]
    [InlineData(-1, 7)]
    [InlineData(8, -1)]
    [InlineData(8, 8)]
    [InlineData(-1, 4)]
    [InlineData(4, -1)]
    [InlineData(8, 4)]
    [InlineData(4, 8)]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(-5, 12)]
    public void SquaresIsInBounds_WithOutOfBoundsSquare_ReturnsFalse(int row, int col)
    {
        // Arrange
        Square square = new(row, col);

        // Act
        var result = Board.IsInBounds(square);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

}
