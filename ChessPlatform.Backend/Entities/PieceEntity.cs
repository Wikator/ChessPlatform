using ChessPlatform.Models.Chess;

namespace ChessPlatform.Backend.Entities;

public class PieceEntity
{
    public Guid Id { get; set; }
    public Guid ChessBoardId { get; set; }
    public ChessBoardEntity? ChessBoard { get; set; }
    public FENChar FENChar { get; set; }
    public Color Color { get; set; }
    public string Type { get; set; }
    public bool? HasMoved { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
}