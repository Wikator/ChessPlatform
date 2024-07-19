namespace ChessPlatform.Models.DTOs;

public class LastMoveDto
{
    public required CoordsDto From { get; init; }
    public required CoordsDto To { get; init; }
    public required DateTime PlayedAt { get; init; }
}
