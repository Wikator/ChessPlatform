using System.Text.Json.Serialization;
using ChessPlatform.ChessLogic.Models;

namespace ChessPlatform.ChessLogic.Pieces;

[method: JsonConstructor]
public record class Bishop(Color Color) : Piece(Color)
{
    public override FENChar FENChar { get; } = Color == Color.White ? FENChar.WhiteBishop : FENChar.BlackBishop;

    public override Coords[] Directions { get; protected set; } =
    [
        new Coords(1, 1),
        new Coords(1, -1),
        new Coords(-1, 1),
        new Coords(-1, -1)
    ];
}