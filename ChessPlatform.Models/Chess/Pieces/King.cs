using System.Text.Json.Serialization;

namespace ChessPlatform.Models.Chess.Pieces;

[method: JsonConstructor]
public record King(Color Color) : Piece(Color)
{
    public override FENChar FENChar { get; } = Color == Color.White ? FENChar.WhiteKing : FENChar.BlackKing;

    public override Coords[] Directions { get; protected set; } =
    [
        new Coords(1, 1),
        new Coords(1, -1),
        new Coords(-1, 1),
        new Coords(-1, -1),
        new Coords(1, 0),
        new Coords(-1, 0),
        new Coords(0, 1),
        new Coords(0, -1)
    ];
}
