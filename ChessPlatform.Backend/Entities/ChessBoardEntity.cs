using ChessPlatform.Models.Chess;

namespace ChessPlatform.Backend.Entities;

public class ChessBoardEntity
{
    public Guid Id { get; set; }
    public Guid? LastMoveId { get; set; }
    public LastMoveEntity? LastMove { get; set; }

    public Color PlayerTurn { get; set; } = Color.White;

    public ICollection<PieceEntity> Pieces { get; set; } = [];
}