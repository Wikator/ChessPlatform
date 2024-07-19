using System.Security.Claims;
using ChessPlatform.Backend.BackgroundServices;
using ChessPlatform.Backend.Configuration;
using ChessPlatform.Backend.Data;
using ChessPlatform.Backend.Services;
using ChessPlatform.Backend.SignalR;
using ChessPlatform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IChessService, ChessService>();
builder.Services.AddSignalR();

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

builder.Services.AddCors();
builder.Services.AddHostedService<TimeoutCheckerService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("chess"));
});

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
if ((await db.Database.GetPendingMigrationsAsync()).Any()) await db.Database.MigrateAsync();


// app.UseHttpsRedirection();

app.UseCors(cors =>
{
    cors.AllowAnyHeader();
    cors.AllowAnyMethod();
    cors.WithOrigins("http://localhost:5284");
    cors.AllowCredentials();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("test", () => "Hello World!").RequireAuthorization();
app.MapGet("account/info", (HttpContext context) =>
{
    var userEmail = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
    var userId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
    
    if (userId is null || userEmail is null)
        return Results.Unauthorized();
    
    return Results.Ok(new UserInfo
    {
        Email = userEmail.Value,
        Id = userId.Value
    });
}).RequireAuthorization();

app.MapGet("game/{id:guid}", async (IChessService chessService, Guid id) =>
{
    var game = await chessService.GetGameDto(id);
    return Results.Ok(game);
});

app.MapPost("game", async (IChessService chessService, HttpContext context) =>
{
    var userId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
    if (userId is null)
        return Results.Unauthorized();

    var gameId = await chessService.CreateGameAsync(userId.Value);
    return Results.Created($"game/{gameId}", gameId);
}).RequireAuthorization();

app.MapIdentityApi<IdentityUser>();

app.MapHub<ChessHub>("/chessHub");

app.Run();
