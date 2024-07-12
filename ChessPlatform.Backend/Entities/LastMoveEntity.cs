namespace ChessPlatform.Backend.Entities;

public class LastMoveEntity
{
    public Guid Id { get; set; }
    public int FromRow { get; set; }
    public int FromColumn { get; set; }
    public int ToRow { get; set; }
    public int ToColumn { get; set; }
}
