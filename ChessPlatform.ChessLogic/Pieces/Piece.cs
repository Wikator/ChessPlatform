using System.Text.Json.Serialization;
using ChessPlatform.ChessLogic.Models;

namespace ChessPlatform.ChessLogic.Pieces;

[method: JsonConstructor]
public abstract record Piece(Color Color)
{

    public abstract FENChar FENChar { get; }
    public abstract Coords[] Directions { get; protected set; }
}
