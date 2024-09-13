using GameLogic.Enums;
using GameLogic.Interfaces;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Client;

public static class Images
{
    #region Fields

    private static readonly Dictionary<PieceType, ImageSource> whiteSources = new()
    {
        [PieceType.Bishop] = LoadImage("../../Assets/WhiteBishop.png"),
        [PieceType.King] = LoadImage("../../Assets/WhiteKing.png"),
        [PieceType.Knight] = LoadImage("../../Assets/WhiteKnight.png"),
        [PieceType.Pawn] = LoadImage("../../Assets/WhitePawn.png"),
        [PieceType.Queen] = LoadImage("../../Assets/WhiteQueen.png"),
        [PieceType.Rook] = LoadImage("../../Assets/WhiteRook.png"),
    };

    private static readonly Dictionary<PieceType, ImageSource> blackSources = new()
    {
        [PieceType.Bishop] = LoadImage("../../Assets/BlackBishop.png"),
        [PieceType.King] = LoadImage("../../Assets/BlackKing.png"),
        [PieceType.Knight] = LoadImage("../../Assets/BlackKnight.png"),
        [PieceType.Pawn] = LoadImage("../../Assets/BlackPawn.png"),
        [PieceType.Queen] = LoadImage("../../Assets/BlackQueen.png"),
        [PieceType.Rook] = LoadImage("../../Assets/BlackRook.png"),
    };

    #endregion



    #region Public Methods

    /// <summary>
    /// Provides the ImageSource that matches the given pieceType and color.
    /// </summary>
    /// <param name="pieceType">The type of piece</param>
    /// <param name="color">The color of the piece</param>
    /// <returns>The ImageSource correspond to the requested piece</returns>
    public static ImageSource GetImageSource(PieceType pieceType, PieceColor color)
    {
        return color switch
        {
            PieceColor.White => whiteSources[pieceType],
            PieceColor.Black => blackSources[pieceType],
            _ => null!
        };
    }


    /// <summary>
    /// Provides the ImageSource that matches the given piece.
    /// </summary>
    /// <param name="piece">An Ipiece</param>
    /// <returns>If a piece is provided, the matching ImageSource. Otherwise, null.</returns>
    public static ImageSource? GetImageSource(IPiece? piece)
    {
        if (piece == null)
        {
            return null;
        }

        return GetImageSource(piece.PieceType, piece.Color);
    }

    #endregion



    #region Private Methods

    private static ImageSource LoadImage(string filepath)
    {
        return new BitmapImage(new Uri(filepath, UriKind.Relative));
    }

    #endregion
}

