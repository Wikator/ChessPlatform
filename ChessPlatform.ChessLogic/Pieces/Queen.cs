using System.Text.Json.Serialization;
using ChessPlatform.ChessLogic.Models;

namespace ChessPlatform.ChessLogic.Pieces;

[method: JsonConstructor]
public record Queen(Color Color) : Piece(Color)
{
    public override FENChar FENChar { get; } = Color == Color.White ? FENChar.WhiteQueen : FENChar.BlackQueen;

    public override Coords[] Directions { get; protected set; } =
    [
        new Coords(1, 1),
        new Coords(1, 0),
        new Coords(1, -1),
        new Coords(0, 1),
        new Coords(0, -1),
        new Coords(-1, 1),
        new Coords(-1, 0),
        new Coords(-1, -1)
    ];
}
