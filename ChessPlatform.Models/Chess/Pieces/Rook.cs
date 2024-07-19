using System.Text.Json.Serialization;

namespace ChessPlatform.Models.Chess.Pieces;

[method: JsonConstructor]
public record Rook(Color Color) : Piece(Color)
{
    public override FENChar FENChar { get; } = Color == Color.White ? FENChar.WhiteRook : FENChar.BlackRook;

    public override Coords[] Directions { get; protected set; } =
    [
        new Coords(1, 0),
        new Coords(-1, 0),
        new Coords(0, 1),
        new Coords(0, -1)
    ];
}
