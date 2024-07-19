using System.ComponentModel.DataAnnotations;
using ChessPlatform.Models.Chess;
using ChessPlatform.Models.Chess.Pieces;
using Microsoft.AspNetCore.Identity;

namespace ChessPlatform.Backend.Entities;

public class ChessGameEntity
{
    public Guid Id { get; set; }
    public string? WhitePlayerId { get; set; }
    public IdentityUser? WhitePlayer { get; set; }
    public string? BlackPlayerId { get; set; }
    public IdentityUser? BlackPlayer { get; set; }
    
    [StringLength(maximumLength: 87, MinimumLength = 23)]
    public required string Fen { get; set; }

    [StringLength(maximumLength: 6)]
    public required string TimeControl { get; init; } = "1+5";
    
    public Color PlayerTurn { get; set; } = Color.White;
    
    public Guid? LastMoveId { get; set; }
    public LastMoveEntity? LastMove { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsOver { get; set; }

    public TimeSpan WhitePlayerRemainingTime { get; set; }
    public TimeSpan BlackPlayerRemainingTime { get; set; }
    
    [StringLength(maximumLength: 50, MinimumLength = 1)]
    public string? GameOverStatus { get; set; }
    public Color? Winner { get; set; }
}