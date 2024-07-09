using System.Security.Claims;
using AutoMapper;
using ChessPlatform.Backend.Configuration;
using ChessPlatform.Backend.Data;
using ChessPlatform.Backend.Services;
using ChessPlatform.Backend.SignalR;
using ChessPlatform.ChessLogic.Pieces;
using ChessPlatform.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IChessService, ChessService>();
builder.Services.AddSignalR();

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

builder.Services.AddCors();

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
app.MapGet("account/email", (HttpContext context) =>
{
    var userEmail = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
    return userEmail?.Value ?? "No email";
}).RequireAuthorization();

app.MapGet("game/{id:int}", (IChessService chessService, IMapper mapper, int id) =>
{
    var game = chessService.GetGame(id);

    return Results.Ok(mapper.Map<GameDto>(game));
});

app.MapIdentityApi<IdentityUser>();

app.MapHub<ChessHub>("/chessHub");

app.Run();
