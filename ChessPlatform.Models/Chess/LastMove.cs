using ChessPlatform.Models.Chess.Pieces;

namespace ChessPlatform.Models.Chess;

public record struct LastMove(Piece Piece, Coords From, Coords To, DateTime PlayedAt);
