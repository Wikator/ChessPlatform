using System.Security.Claims;
using ChessPlatform.Backend.Services;
using ChessPlatform.ChessLogic.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChessPlatform.Backend.SignalR;

public class ChessHub(IChessService chessService) : Hub
{
    public async Task SendMove(int fromRow, int fromCol, int toRow, int toCol)
    {
        var context = Context.GetHttpContext();
        var userId = context?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        
        if (userId is null)
            return;
        
        var game = chessService.GetGame(1);

        switch (game.PlayerColor)
        {
            case Color.Black when userId.Value != game.BlackPlayerId:
                return;
            case Color.White when userId.Value != game.WhitePlayerId:
                return;
        }
        
        chessService.PlayMove(1, new Coords(fromRow, fromCol), new Coords(toRow, toCol));
        await Clients.Others.SendAsync("ReceiveMove", fromRow, fromCol, toRow, toCol);
    }

    public override async Task OnConnectedAsync()
    {
        var context = Context.GetHttpContext();

        if (context is null)
            return;

        var userId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        
        var game = chessService.GetGame(1);
        
        if (game.WhitePlayerId is null)
        {
            game.WhitePlayerId = userId?.Value;
            await Clients.Caller.SendAsync("SetPlayerColor", Color.White);
        }
        else if (game.BlackPlayerId is null)
        {
            game.BlackPlayerId = userId?.Value;
            await Clients.Caller.SendAsync("SetPlayerColor", Color.Black);
        }
        
        await base.OnConnectedAsync();
    }
}