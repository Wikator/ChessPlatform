using ChessPlatform.ChessLogic;
using ChessPlatform.ChessLogic.Models;

namespace ChessPlatform.Backend.Services;

public interface IChessService
{
    public void PlayMove(int gameId, Coords from, Coords to);
    public ChessBoard GetGame(int gameId);
}