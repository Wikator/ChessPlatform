using ChessPlatform.Models.Chess;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChessPlatform.Frontend.Client.Services;

public class SignalRChessService
{
    private readonly HubConnection _hubConnection;
    private bool _isConnected;
    
    private string? _roomId;

    public event Action<Coords, Coords> MoveReceived;
    public event Action<Color> SetPlayerColor; 

    public SignalRChessService()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"http://localhost:5133/chessHub")
            .Build();

        _hubConnection.On<int, int, int, int>("ReceiveMove", (fromRow, fromColumn, toRow, toColumn) =>
        {
            MoveReceived?.Invoke(new Coords(fromRow, fromColumn), new Coords(toRow, toColumn));
        });
        
        _hubConnection.On<Color>("SetPlayerColor", color =>
        {
            SetPlayerColor?.Invoke(color);
        });
    }
    
    public async Task ChangeRoomAsync(string roomId)
    {
        if (!_isConnected)
            return;
        
        if (_roomId is not null)
            await _hubConnection.SendAsync("LeaveRoom", _roomId);
        
        _roomId = roomId;
        await _hubConnection.SendAsync("JoinRoom", roomId);

    }

    public async Task StartAsync()
    {
        if (_isConnected)
            return;
        
        await _hubConnection.StartAsync();
        _isConnected = true;
    }

    public async Task SendMoveAsync(Coords from, Coords to)
    {
        if (_roomId is not null)
            await _hubConnection.InvokeAsync("SendMove", from.Row, from.Column, to.Row, to.Column, _roomId);
    }
}