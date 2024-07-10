using System.Text.Json.Serialization;

namespace ChessPlatform.Models.Chess.Pieces;

[method: JsonConstructor]
public record Knight(Color Color) : Piece(Color)
{
    public override FENChar FENChar { get; } = Color == Color.White ? FENChar.WhiteKnight : FENChar.BlackKnight;

    public override Coords[] Directions { get; protected set; } =
    [
        new Coords(1, 2),
        new Coords(2, 1),
        new Coords(2, -1),
        new Coords(1, -2),
        new Coords(-1, -2),
        new Coords(-2, -1),
        new Coords(-2, 1),
        new Coords(-1, 2)
    ];
}
