using FluentAssertions;
using GameLogic;
using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Interfaces;
using GameLogic.Moves;
using GameLogic.Pieces;

namespace GameLogicTests;

public class GameManagerTests
{
    #region GameIsOver Tests

    [Fact]
    public void GameIsOver_WithAvailableMoves_ReturnsFalse()
    {
        // Arrange
        Board board = new();
        board.Initialize();

        GameManager gameManager = new(board, PieceColor.White);

        // Act
        var result = gameManager.GameIsOver();

        // Assert
        result.Should().BeFalse();
    }


    [Fact]
    public void GameIsOver_WithOnlyKings_ReturnsTrue()
    {
        // Arrange
        Board board = new();
        board.AddNewPiece<KingPiece>(StartSquares.WhiteKing, PieceColor.White);
        board.AddNewPiece<KingPiece>(StartSquares.BlackKing, PieceColor.Black);

        GameManager gameManager = new(board, PieceColor.White);

        // Act
        var result = gameManager.GameIsOver();

        // Assert
        result.Should().BeTrue();
    }


    [Fact]
    public void GameIsOver_WhiteUnderCheckmate_ReturnsTrue()
    {
        // Arrange
        Board board = new();
        board.AddNewPiece<KingPiece>(StartSquares.WhiteKing, PieceColor.White);
        board.AddNewPiece<KingPiece>(StartSquares.BlackKing, PieceColor.Black);
        board.AddNewPiece<RookPiece>((7, 7), PieceColor.Black);
        board.AddNewPiece<RookPiece>((6, 7), PieceColor.Black);

        GameManager gameManager = new(board, PieceColor.White);

        // Act
        var result = gameManager.GameIsOver();

        // Assert
        result.Should().BeTrue();
    }


    [Fact]
    public void GameIsOver_WhiteTurnNoValidMoves_ReturnsTrue()
    {
        // Arrange
        Board board = new();
        board.AddNewPiece<KingPiece>((0, 0), PieceColor.White);
        board.AddNewPiece<KingPiece>((7, 7), PieceColor.Black);
        board.AddNewPiece<RookPiece>((1, 7), PieceColor.Black);
        board.AddNewPiece<RookPiece>((7, 1), PieceColor.Black);

        GameManager gameManager = new(board, PieceColor.White);

        // Act
        var result = gameManager.GameIsOver();

        // Assert
        result.Should().BeTrue();
    }

    #endregion



    #region GetGameResult Tests

    [Fact]
    public void GetGameResult_GameNotOver_ThrowsException()
    {
        // Arrange
        Board board = new();
        board.Initialize();

        GameManager gameManager = new(board, PieceColor.White);

        // Act + Assert
        Assert.Throws<Exception>(() => gameManager.GetGameResult());
    }


    [Fact]
    public void GetGameResult_WhiteWinsByCheckmate()
    {
        // Game ends when black has no available moves at start of turn

        // Arrange
        Board board = new();
        board.AddNewPiece<KingPiece>((0, 0), PieceColor.Black);
        board.AddNewPiece<KingPiece>((7, 7), PieceColor.White);
        board.AddNewPiece<RookPiece>((0, 7), PieceColor.White);
        board.AddNewPiece<RookPiece>((1, 7), PieceColor.White);

        GameManager gameManager = new(board, PieceColor.White);
        gameManager.SwitchTurn();

        // Act
        var (winner, reason) = gameManager.GetGameResult();

        // Assert
        winner.Should().Be(PieceColor.White);
        reason.Should().Be(GameOverReason.Checkmate);
    }


    [Fact]
    public void GetGameResult_BlackWinsByCheckmate()
    {
        // Game ends when white has no available moves at start of turn

        // Arrange
        Board board = new();
        board.AddNewPiece<KingPiece>((0, 0), PieceColor.White);
        board.AddNewPiece<KingPiece>((7, 7), PieceColor.Black);
        board.AddNewPiece<RookPiece>((0, 7), PieceColor.Black);
        board.AddNewPiece<RookPiece>((1, 7), PieceColor.Black);

        GameManager gameManager = new(board, PieceColor.White);

        // Act
        var (winner, reason) = gameManager.GetGameResult();

        // Assert
        winner.Should().Be(PieceColor.Black);
        reason.Should().Be(GameOverReason.Checkmate);
    }


    [Fact]
    public void GetGameResult_WhiteTurnStalemate()
    {
        // Game ends when white has no available moves at start of turn

        // Arrange
        Board board = new();
        board.AddNewPiece<KingPiece>((0, 0), PieceColor.White);
        board.AddNewPiece<KingPiece>((7, 7), PieceColor.Black);
        board.AddNewPiece<RookPiece>((1, 7), PieceColor.Black);
        board.AddNewPiece<RookPiece>((7, 1), PieceColor.Black);

        GameManager gameManager = new(board, PieceColor.White);

        // Act
        var (winner, reason) = gameManager.GetGameResult();

        // Assert
        winner.Should().Be(null);
        reason.Should().Be(GameOverReason.Stalemate);
    }


    [Fact]
    public void GetGameResult_BlackTurnStalemate()
    {
        // Game ends when black has no available moves at start of turn

        // Arrange
        Board board = new();
        board.AddNewPiece<KingPiece>((0, 0), PieceColor.Black);
        board.AddNewPiece<KingPiece>((7, 7), PieceColor.White);
        board.AddNewPiece<RookPiece>((1, 7), PieceColor.White);
        board.AddNewPiece<RookPiece>((7, 1), PieceColor.White);

        GameManager gameManager = new(board, PieceColor.White);
        gameManager.SwitchTurn();

        // Act
        var (winner, reason) = gameManager.GetGameResult();

        // Assert
        winner.Should().Be(null);
        reason.Should().Be(GameOverReason.Stalemate);
    }


    [Fact]
    public void GetGameResult_WhiteTurnInsufficientMaterial()
    {
        // Arrange
        Board board = new();
        board.AddNewPiece<KingPiece>(StartSquares.WhiteKing, PieceColor.White);
        board.AddNewPiece<KingPiece>(StartSquares.BlackKing, PieceColor.Black);

        GameManager gameManager = new(board, PieceColor.White);

        // Act
        var (winner, reason) = gameManager.GetGameResult();

        // Assert
        winner.Should().Be(null);
        reason.Should().Be(GameOverReason.InsufficientMaterial);
    }


    [Fact]
    public void GetGameResult_BlackTurnInsufficientMaterial()
    {
        // Arrange
        Board board = new();
        board.AddNewPiece<KingPiece>(StartSquares.WhiteKing, PieceColor.White);
        board.AddNewPiece<KingPiece>(StartSquares.BlackKing, PieceColor.Black);

        GameManager gameManager = new(board, PieceColor.White);
        gameManager.SwitchTurn();

        // Act
        var (winner, reason) = gameManager.GetGameResult();

        // Assert
        winner.Should().Be(null);
        reason.Should().Be(GameOverReason.InsufficientMaterial);
    }

    #endregion



    #region GetPlayerMoves Tests

    [Theory]
    [InlineData(PieceColor.White)]
    [InlineData(PieceColor.Black)]
    public void GetPlayerMoves_White_FillsWhiteSquares(PieceColor playerColor)
    {
        // This test checks that moves are generated on the squares corresponding
        // to those that hold the player's pieces on the board

        // Arrange
        Board board = new();
        board.Initialize();

        GameManager gameManager = new(board, PieceColor.White);

        // Act
        var result = gameManager.GetPlayerMoves(playerColor);

        // Assert
        for (int r = 0; r < Board.BoardSize; r++)
        {
            for (int c = 0; c < Board.BoardSize; c++)
            {
                var piece = gameManager.Board.State[r, c];
                if (piece == null ||
                    piece.Color != playerColor)
                {
                    result[r, c].Should().BeNull();
                }
                else
                {
                    result[r, c].Should().NotBeNull();
                }
            }
        }
    }

    #endregion
}

