using ChessPlatform.ChessLogic;
using ChessPlatform.ChessLogic.ChessBoard;
using ChessPlatform.Models.Chess;

namespace ChessPlatform.Backend.Services;

public class ChessService : IChessService
{
    private int _nextGameId = 2;
    
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

    public int CreateGame(string whitePlayerId, string? blackPlayerId = null)
    {
        var gameId = _nextGameId++;
        
        var game = new ChessBoard
        {
            WhitePlayerId = whitePlayerId,
            BlackPlayerId = blackPlayerId
        };
        
        _boards.Add(gameId, game);
        return gameId;
    }
}
