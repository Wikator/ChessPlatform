using ChessPlatform.ChessLogic.ChessBoard;
using ChessPlatform.Models.Chess;
using ChessPlatform.Models.DTOs;

namespace ChessPlatform.Backend.Services;

public interface IChessService
{
    public Task PlayMove(Guid gameId, Coords from, Coords to, FENChar? promotedType);
    public Task<ChessBoard?> GetGame(Guid gameId);
    public Task<GameDto?> GetGameDto(Guid gameId);
    public Task<Guid> CreateGameAsync(string whitePlayerId, string? blackPlayerId = null);
    public Task SetWhitePlayer(Guid gameId, string whitePlayerId);
    public Task SetBlackPlayer(Guid gameId, string blackPlayerId);
}