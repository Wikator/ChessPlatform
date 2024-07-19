using ChessPlatform.Models.Chess;

namespace ChessPlatform.Models.DTOs;

public class GameDto
{
    public Guid Id { get; init; }
    public UserDto? WhitePlayer { get; init; }
    public UserDto? BlackPlayer { get; init; }
    public required string Fen { get; init; }
    public required string TimeControl { get; init; }
    public TimeSpan WhitePlayerRemainingTime { get; init; }
    public TimeSpan BlackPlayerRemainingTime { get; init; }
    public Color PlayerTurn { get; init; } = Color.White;
    public LastMoveDto? LastMove { get; init; }
    public bool IsOver { get; init; }
    public string? GameOverStatus { get; init; }
    public Color? Winner { get; init; }
}