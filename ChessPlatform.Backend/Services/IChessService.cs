using ChessPlatform.ChessLogic.ChessBoard;
using ChessPlatform.Models.Chess;

namespace ChessPlatform.Backend.Services;

public interface IChessService
{
    public void PlayMove(int gameId, Coords from, Coords to);
    public ChessBoard GetGame(int gameId);
    public int CreateGame(string whitePlayerId, string? blackPlayerId = null);
}