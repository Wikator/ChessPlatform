namespace ChessPlatform.Models.DTOs;

public class LastMoveDto
{
    public required PieceDto Piece { get; set; }
    public required CoordsDto From { get; set; }
    public required CoordsDto To { get; set; }
}
