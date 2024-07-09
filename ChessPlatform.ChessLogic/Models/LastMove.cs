using ChessPlatform.ChessLogic.Pieces;

namespace ChessPlatform.ChessLogic.Models;

public record struct LastMove(Piece Piece, Coords From, Coords To);