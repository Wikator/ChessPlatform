using ChessPlatform.ChessLogic.Models;

namespace ChessPlatform.Shared;

public class PieceDto
{
    public FENChar FENChar { get; set; }
    public Color Color { get; set; }
    public required string Type { get; set; }
    public bool? HasMoved { get; set; }
}