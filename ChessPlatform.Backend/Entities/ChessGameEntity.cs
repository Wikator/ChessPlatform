using Microsoft.AspNetCore.Identity;

namespace ChessPlatform.Backend.Entities;

public class ChessGameEntity
{
    public Guid Id { get; set; }
    public Guid ChessBoardId { get; set; }
    public ChessBoardEntity? ChessBoard { get; set; }
    
    public string? WhitePlayerId { get; set; }
    public IdentityUser? WhitePlayer { get; set; }
    public string? BlackPlayerId { get; set; }
    public IdentityUser? BlackPlayer { get; set; }
}