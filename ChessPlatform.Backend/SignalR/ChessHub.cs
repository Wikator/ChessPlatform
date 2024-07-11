using System.Security.Claims;
using ChessPlatform.Backend.Services;
using ChessPlatform.Models.Chess;
using Microsoft.AspNetCore.SignalR;

namespace ChessPlatform.Backend.SignalR;

public class ChessHub(IChessService chessService) : Hub
{
    public async Task SendMove(int fromRow, int fromCol, int toRow, int toCol, int roomId)
    {
        var context = Context.GetHttpContext();
        var userId = context?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        
        if (userId is null)
            return;
        
        var game = chessService.GetGame(roomId);

        switch (game.PlayerColor)
        {
            case Color.Black when userId.Value != game.BlackPlayerId:
                return;
            case Color.White when userId.Value != game.WhitePlayerId:
                return;
        }
        
        chessService.PlayMove(roomId, new Coords(fromRow, fromCol), new Coords(toRow, toCol));
        await Clients.OthersInGroup(roomId.ToString()).SendAsync("ReceiveMove", fromRow, fromCol, toRow, toCol);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.Others.SendAsync("PlayerLeft", Context.ConnectionId);
    }
    
    public async Task JoinRoom(string roomId)
    {
        var context = Context.GetHttpContext();

        if (context is null)
            return;
        
        if (!int.TryParse(roomId, out var id))
            return;
        
        var game = chessService.GetGame(id);
        var userId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        
        if (userId is null)
            return;
        
        if (game.WhitePlayerId is null)
        {
            game.WhitePlayerId = userId.Value;
        }
        else if (game.BlackPlayerId is null && game.WhitePlayerId != userId.Value)
        {
            game.BlackPlayerId = userId.Value;
        }
        
        if (game.WhitePlayerId is not null && game.WhitePlayerId == userId?.Value)
            await Clients.Caller.SendAsync("SetPlayerColor", Color.White);
        else if (game.BlackPlayerId is not null && game.BlackPlayerId == userId?.Value)
            await Clients.Caller.SendAsync("SetPlayerColor", Color.Black);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Clients.OthersInGroup(roomId).SendAsync("PlayerJoined");
    }
    
    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        await Clients.OthersInGroup(roomId).SendAsync("PLayerLeft");
    }
}