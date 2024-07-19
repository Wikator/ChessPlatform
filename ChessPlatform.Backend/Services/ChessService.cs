using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChessPlatform.Backend.Data;
using ChessPlatform.Backend.Entities;
using ChessPlatform.ChessLogic.ChessBoard;
using ChessPlatform.Models.Chess;
using ChessPlatform.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ChessPlatform.Backend.Services;

public class ChessService(ApplicationDbContext context, IMapper mapper) : IChessService
{
    public async Task PlayMove(Guid gameId, Coords from, Coords to, FENChar? promotedType)
    {
        var chessGame = await context.ChessGames
            .Where(game => game.Id == gameId)
            .Include(game => game.LastMove)
            .Include(chessBoard => chessBoard.WhitePlayer)
            .SingleOrDefaultAsync();
        
        if (chessGame is null)
            return;
        
        var board = mapper.Map<ChessBoard>(chessGame);
        
        board.Move(from, to, promotedType);
        mapper.Map(board, chessGame);
        
        context.Update(chessGame);
        await context.SaveChangesAsync();
    }

    public async Task<ChessBoard?> GetGame(Guid gameId)
    {
        var game = await context.ChessGames
            .Where(game => game.Id == gameId)
            .Include(chessBoard => chessBoard.LastMove)
            .SingleOrDefaultAsync();
        
        return mapper.Map<ChessBoard?>(game);
    }

    public async Task<GameDto?> GetGameDto(Guid gameId)
    {
        return await context.ChessGames
            .Where(game => game.Id == gameId)
            .ProjectTo<GameDto>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    public async Task<Guid> CreateGameAsync(string whitePlayerId, string? blackPlayerId = null)
    {
        var chessBoard = new ChessBoard
        {
            WhitePlayerId = whitePlayerId,
            BlackPlayerId = blackPlayerId
        };
        
        var chessGameEntity = mapper.Map<ChessGameEntity>(chessBoard);
        
        Console.WriteLine(chessGameEntity.Fen);
        var playerTimes = TimeSpan.FromMinutes(int.Parse(chessGameEntity.TimeControl.Split('+').First()));
        chessGameEntity.WhitePlayerRemainingTime = playerTimes;
        chessGameEntity.BlackPlayerRemainingTime = playerTimes;
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
