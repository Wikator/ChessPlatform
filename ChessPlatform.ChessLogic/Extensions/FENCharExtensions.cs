using ChessPlatform.Models.Chess;

namespace ChessPlatform.ChessLogic.Extensions;

// ReSharper disable once InconsistentNaming
public static class FENCharExtensions
{
    public static string ToFenString(this FENChar fenChar) =>
        ((char)fenChar).ToString();
}