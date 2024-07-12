using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChessPlatform.Backend.Data;
using ChessPlatform.Backend.Entities;
using ChessPlatform.ChessLogic;
using ChessPlatform.ChessLogic.ChessBoard;
using ChessPlatform.Models.Chess;
using Microsoft.EntityFrameworkCore;

namespace ChessPlatform.Backend.Services;

public class ChessService(ApplicationDbContext context, IMapper mapper) : IChessService
{
    public async Task PlayMove(Guid gameId, Coords from, Coords to)
    {
        var chessGame = await context.ChessGames
            .Where(game => game.Id == gameId)
            .Include(game => game.ChessBoard)
            .ThenInclude(chessBoard => chessBoard!.Pieces)
            .Include(game => game.ChessBoard)
            .ThenInclude(chessBoard => chessBoard!.LastMove)
            .Include(chessBoard => chessBoard.WhitePlayer)
            .SingleOrDefaultAsync();
        
        if (chessGame is null)
            return;
        
        var board = mapper.Map<ChessBoard>(chessGame);

        board?.Move(from, to);
        context.ChessGames.Remove(chessGame);
        await context.SaveChangesAsync();
        chessGame.ChessBoard = mapper.Map<ChessBoardEntity>(board);
        context.ChessGames.Add(chessGame);
        await context.SaveChangesAsync();
    }

    public async Task<ChessBoard?> GetGame(Guid gameId)
    {
        var game = await context.ChessGames
            .Where(game => game.Id == gameId)
            .Include(game => game.ChessBoard)
            .ThenInclude(chessBoard => chessBoard!.Pieces)
            .Include(game => game.ChessBoard)
            .ThenInclude(chessBoard => chessBoard!.LastMove)
            .Include(chessBoard => chessBoard.WhitePlayer)
            .SingleOrDefaultAsync();
        
        return mapper.Map<ChessBoard?>(game);
    }

    public async Task<Guid> CreateGameAsync(string whitePlayerId, string? blackPlayerId = null)
    {
        var chessBoard = new ChessBoard
        {
            WhitePlayerId = whitePlayerId,
            BlackPlayerId = blackPlayerId
        };
        
        var chessBoardEntity = mapper.Map<ChessBoardEntity>(chessBoard);
        
        var chessGameEntity = new ChessGameEntity
        {
            ChessBoard = chessBoardEntity,
            WhitePlayerId = whitePlayerId,
            BlackPlayerId = blackPlayerId
        };
        
        context.ChessGames.Add(chessGameEntity);
        await context.SaveChangesAsync();
        return chessGameEntity.Id;
    }

    public async Task SetWhitePlayer(Guid gameId, string whitePlayerId)
    {
        var game = await context.ChessGames.FindAsync(gameId);
        game!.WhitePlayerId = whitePlayerId;
        await context.SaveChangesAsync();
    }

    public async Task SetBlackPlayer(Guid gameId, string blackPlayerId)
    {
        var game = await context.ChessGames.FindAsync(gameId);
        game!.BlackPlayerId = blackPlayerId;
        await context.SaveChangesAsync();
    }
}
