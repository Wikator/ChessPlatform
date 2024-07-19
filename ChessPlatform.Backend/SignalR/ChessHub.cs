using System.Security.Claims;
using ChessPlatform.Backend.Services;
using ChessPlatform.Models.Chess;
using ChessPlatform.Models.Chess.Pieces;
using Microsoft.AspNetCore.SignalR;

namespace ChessPlatform.Backend.SignalR;

public class ChessHub(IChessService chessService) : Hub
{
    public async Task SendMove(int fromRow, int fromCol, int toRow, int toCol,
        FENChar? promotedType, string roomId)
    {
        var context = Context.GetHttpContext();
        var userId = context?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        
        if (userId is null)
            return;
        
        var game = await chessService.GetGame(new Guid(roomId));
        
        if (game is null)
            return;
    
        switch (game.PlayerTurn)
        {
            case Color.Black when userId.Value != game.BlackPlayerId:
                return;
            case Color.White when userId.Value != game.WhitePlayerId:
                return;
        }
        
        var isPromotionMove = game[fromRow, fromCol] is Pawn && toRow is 0 or 7;

        if (isPromotionMove)
        {
            promotedType ??= game.PlayerTurn switch
            {
                Color.White => FENChar.WhiteQueen,
                Color.Black => FENChar.BlackQueen,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        else
        {
            promotedType = null;
        }
        
        await chessService.PlayMove(new Guid(roomId), new Coords(fromRow, fromCol),
            new Coords(toRow, toCol), promotedType);
        
        await Clients.Groups(roomId).SendAsync("RemainingTime", game.WhitePlayerRemainingTime.TotalSeconds,
            game.BlackPlayerRemainingTime.TotalSeconds);
        
        await Clients.OthersInGroup(roomId).SendAsync("ReceiveMove", fromRow, fromCol, toRow,
            toCol, isPromotionMove ? promotedType : null);
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
        
        var game = await chessService.GetGame(new Guid(roomId));
        
        if (game is null)
            return;
        
        var userId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        
        if (userId is null)
            return;
        
        if (game.WhitePlayerId is null)
        {
            await chessService.SetWhitePlayer(new Guid(roomId), userId.Value);
        }
        else if (game.BlackPlayerId is null && game.WhitePlayerId != userId.Value)
        {
            await chessService.SetBlackPlayer(new Guid(roomId), userId.Value);
        }
        
        if (game.WhitePlayerId is not null && game.WhitePlayerId == userId.Value)
            await Clients.Caller.SendAsync("SetPlayerColor", Color.White);
        else if (game.BlackPlayerId is not null && game.BlackPlayerId == userId.Value)
            await Clients.Caller.SendAsync("SetPlayerColor", Color.Black);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Clients.OthersInGroup(roomId).SendAsync("PlayerJoined");
    }
    
    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        await Clients.OthersInGroup(roomId).SendAsync("PlayerLeft");
    }
}