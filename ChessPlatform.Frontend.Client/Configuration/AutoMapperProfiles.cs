using AutoMapper;
using ChessPlatform.ChessLogic;
using ChessPlatform.Models.Chess;
using ChessPlatform.Models.DTOs;

namespace ChessPlatform.Frontend.Client.Configuration;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<GameDto, ChessLogic.ChessBoard.ChessBoard>()
            .ConvertUsing(new ChessGameDtoEntityConverter());
    }
    
    private class ChessGameDtoEntityConverter : ITypeConverter<GameDto, ChessLogic.ChessBoard.ChessBoard>
    {
        public ChessLogic.ChessBoard.ChessBoard Convert(GameDto source, ChessLogic.ChessBoard.ChessBoard destination,
            ResolutionContext context)
        {
            var pieces = FenConverter.ConvertFenToBoard(source.Fen);
            
            var castlingRights = source.Fen.Split(' ')[2];
            var canWhiteCastleKingSide = castlingRights.Contains('K');
            var canWhiteCastleQueenSide = castlingRights.Contains('Q');
            var canBlackCastleKingSide = castlingRights.Contains('k');
            var canBlackCastleQueenSide = castlingRights.Contains('q');
            
            LastMove? lastMove = null;
            
            if (source.LastMove is not null)
            {
                lastMove = new LastMove(pieces[source.LastMove.To.Row, source.LastMove.To.Column]!,
                    new Coords(source.LastMove.From.Row, source.LastMove.From.Column),
                    new Coords(source.LastMove.To.Row, source.LastMove.To.Column),
                    source.LastMove.PlayedAt);
            }
            
            var game = new ChessLogic.ChessBoard.ChessBoard(pieces, source.PlayerTurn, lastMove, source.TimeControl,
                source.IsOver, source.WhitePlayerRemainingTime, source.BlackPlayerRemainingTime,
                canWhiteCastleKingSide, canWhiteCastleQueenSide, canBlackCastleKingSide, canBlackCastleQueenSide)
            {
                WhitePlayerId = source.WhitePlayer?.Id,
                BlackPlayerId = source.BlackPlayer?.Id
            };
            return game;
        }
    }
}