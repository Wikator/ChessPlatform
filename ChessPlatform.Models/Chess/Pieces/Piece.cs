using System.Text.Json.Serialization;

namespace ChessPlatform.Models.Chess.Pieces;

[method: JsonConstructor]
public abstract record Piece(Color Color)
{

    public abstract FENChar FENChar { get; }
    public abstract Coords[] Directions { get; protected set; }
}
