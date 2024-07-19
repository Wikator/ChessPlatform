using AutoMapper;
using ChessPlatform.Backend.Entities;
using ChessPlatform.ChessLogic;
using ChessPlatform.ChessLogic.ChessBoard;
using ChessPlatform.Models.Chess;
using ChessPlatform.Models.Chess.Pieces;
using ChessPlatform.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace ChessPlatform.Backend.Configuration;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<ChessGameEntity, GameDto>();
        CreateMap<IdentityUser, UserDto>();
        CreateMap<ChessBoard, GameDto>();
        CreateMap<LastMoveEntity, LastMoveDto>()
            .ForMember(dest => dest.From,
                opt => opt.MapFrom(src => new CoordsDto { Row = src.FromRow, Column = src.FromColumn }))
            .ForMember(dest => dest.To,
                opt => opt.MapFrom(src => new CoordsDto { Row = src.ToRow, Column = src.ToColumn }));
        CreateMap<LastMove, LastMoveDto>();
        
        CreateMap<LastMove, LastMoveEntity>()
            .ForMember(dest => dest.FromRow, opt => opt.MapFrom(src => src.From.Row))
            .ForMember(dest => dest.FromColumn, opt => opt.MapFrom(src => src.From.Column))
            .ForMember(dest => dest.ToRow, opt => opt.MapFrom(src => src.To.Row))
            .ForMember(dest => dest.ToColumn, opt => opt.MapFrom(src => src.To.Column));

        CreateMap<ChessBoard, ChessGameEntity>()
            .ForMember(dest => dest.Fen, opt => opt.MapFrom(src => src.BoardAsFen));
        
        CreateMap<ChessGameEntity, ChessBoard>()
            .ConvertUsing<ChessGameEntityConverter>();
    }
    
    // ReSharper disable once ClassNeverInstantiated.Local
    private class ChessGameEntityConverter : ITypeConverter<ChessGameEntity, ChessBoard>
    {
        public ChessBoard Convert(ChessGameEntity source, ChessBoard destination, ResolutionContext context)
        {
            var pieces = FenConverter.ConvertFenToBoard(source.Fen);
            var lastMove = GetLastMove(source.LastMove, pieces);
            
            var castlingRights = source.Fen.Split(' ')[2];
            var canWhiteCastleKingSide = castlingRights.Contains('K');
            var canWhiteCastleQueenSide = castlingRights.Contains('Q');
            var canBlackCastleKingSide = castlingRights.Contains('k');
            var canBlackCastleQueenSide = castlingRights.Contains('q');
            
            var game = new ChessBoard(pieces, source.PlayerTurn, lastMove, source.TimeControl, source.IsOver,
                source.WhitePlayerRemainingTime, source.BlackPlayerRemainingTime,
                canWhiteCastleKingSide, canWhiteCastleQueenSide, canBlackCastleKingSide, canBlackCastleQueenSide)
            {
                WhitePlayerId = source.WhitePlayerId,
                BlackPlayerId = source.BlackPlayerId
            };
            return game;
        }

        
        private static LastMove? GetLastMove(LastMoveEntity? lastMoveEntity, Piece?[,] pieces)
        {
            if (lastMoveEntity is null)
                return null;
            
            var from = new Coords(lastMoveEntity.FromRow, lastMoveEntity.FromColumn);
            var to = new Coords(lastMoveEntity.ToRow, lastMoveEntity.ToColumn);
            
            return new LastMove(pieces[to.Row, to.Column]!, from, to, lastMoveEntity.PlayedAt);
        }
    }
}