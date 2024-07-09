using ChessPlatform.ChessLogic.Models;

namespace ChessPlatform.Shared;

public class GameDto
{
    public PieceDto?[][] Board { get; init; }
    public Color PlayerColor { get; init; }
    public LastMoveDto? LastMove { get; init; }
    public string? WhitePlayerId { get; init; }
    public string? BlackPlayerId { get; init; }
}