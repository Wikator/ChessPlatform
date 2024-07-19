using ChessPlatform.Backend.Data;
using ChessPlatform.Models.Chess;
using Microsoft.EntityFrameworkCore;

namespace ChessPlatform.Backend.BackgroundServices;

public class TimeoutCheckerService(IServiceScopeFactory scopeFactory) : BackgroundService
{
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckTimeoutsAsync();
            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task CheckTimeoutsAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var activeGames = await context.ChessGames
            .Where(g => g.GameOverStatus == null)
            .Include(g => g.LastMove)
            .ToListAsync();

        foreach (var game in activeGames)
        {
            var timeSinceLastMove = DateTime.UtcNow - (game.LastMove?.PlayedAt ?? game.CreatedAt);
            
            switch (game.PlayerTurn)
            {
                case Color.White when game.WhitePlayerRemainingTime <= timeSinceLastMove:
                    game.GameOverStatus = "Timeout";
                    game.Winner = Color.Black;
                    game.IsOver = true;
                    game.WhitePlayerRemainingTime = TimeSpan.Zero;
                    break;
                case Color.Black when game.BlackPlayerRemainingTime <= timeSinceLastMove:
                    game.GameOverStatus = "Timeout";
                    game.Winner = Color.White;
                    game.IsOver = true;
                    game.BlackPlayerRemainingTime = TimeSpan.Zero;
                    break;
            }
        }

        await context.SaveChangesAsync();
    }
}
