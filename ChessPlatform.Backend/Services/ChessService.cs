using ChessPlatform.ChessLogic;
using ChessPlatform.ChessLogic.Models;

namespace ChessPlatform.Backend.Services;

public class ChessService : IChessService
{
    private readonly Dictionary<int, ChessBoard> _boards = new()
    {
        { 1, new ChessBoard() }
    };
    
    public void PlayMove(int gameId, Coords from, Coords to)
    {
        var board = _boards[gameId];
        board.Move(from, to);
    }

    public ChessBoard GetGame(int gameId)
    {
        return _boards[gameId];
    }
}
