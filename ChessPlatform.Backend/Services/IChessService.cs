using ChessPlatform.ChessLogic.ChessBoard;
using ChessPlatform.Models.Chess;

namespace ChessPlatform.Backend.Services;

public interface IChessService
{
    public Task PlayMove(Guid gameId, Coords from, Coords to);
    public Task<ChessBoard?> GetGame(Guid gameId);
    public Task<Guid> CreateGameAsync(string whitePlayerId, string? blackPlayerId = null);
    public Task SetWhitePlayer(Guid gameId, string whitePlayerId);
    public Task SetBlackPlayer(Guid gameId, string blackPlayerId);
}