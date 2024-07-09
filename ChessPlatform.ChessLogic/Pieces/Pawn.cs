using System.Text.Json.Serialization;
using ChessPlatform.ChessLogic.Models;

namespace ChessPlatform.ChessLogic.Pieces;

[method: JsonConstructor]
public record Pawn(Color Color) : Piece(Color)
{
    public bool HasMoved { get; private set; }
    
    public override FENChar FENChar { get; } = Color == Color.White ? FENChar.WhitePawn : FENChar.BlackPawn;

    public override Coords[] Directions { get; protected set; } =
        Color switch
        {
            Color.White =>
            [
                new Coords(1, 0),
                new Coords(2, 0),
                new Coords(1, 1),
                new Coords(1, -1)
            ],
            Color.Black =>
            [
                new Coords(-1, 0),
                new Coords(-2, 0),
                new Coords(-1, 1),
                new Coords(-1, -1)
            ],
            _ => throw new ArgumentOutOfRangeException(nameof(Color), Color, null)
        };

    public void SetHasMoved()
    {
        HasMoved = true;
        
        Directions = Color switch
        {
            Color.White =>
            [
                new Coords(1, 0),
                new Coords(1, 1),
                new Coords(1, -1)
            ],
            Color.Black =>
            [
                new Coords(-1, 0),
                new Coords(-1, 1),
                new Coords(-1, -1)
            ],
            _ => throw new ArgumentOutOfRangeException(nameof(Color), Color, null)
        };
    }
}