namespace ChessPlatform.Models.Chess;

public record struct Square(FENChar? Piece, Coords Coords);
