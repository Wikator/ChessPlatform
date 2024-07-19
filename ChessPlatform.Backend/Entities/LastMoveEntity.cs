using System.ComponentModel.DataAnnotations.Schema;

namespace ChessPlatform.Backend.Entities;

[Table("LastMoves")]
public class LastMoveEntity
{
    public Guid Id { get; set; }
    public int FromRow { get; init; }
    public int FromColumn { get; init; }
    public int ToRow { get; init; }
    public int ToColumn { get; init; }
    public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
}
