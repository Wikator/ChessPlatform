namespace ChessPlatform.ChessLogic.Models;

public record struct Square(FENChar? Piece, Coords Coords);
