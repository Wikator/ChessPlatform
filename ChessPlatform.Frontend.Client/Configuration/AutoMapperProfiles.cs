using AutoMapper;
using ChessPlatform.ChessLogic.Models;
using ChessPlatform.ChessLogic.Pieces;
using ChessPlatform.Shared;

namespace ChessPlatform.Frontend.Client.Configuration;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<PieceDto, Piece>()
            .ConvertUsing<PieceDtoToPieceConverter>();

        CreateMap<CoordsDto, Coords>();
        CreateMap<LastMoveDto, LastMove>();
    }
    
    private class PieceDtoToPieceConverter : ITypeConverter<PieceDto, Piece>
    {
        public Piece Convert(PieceDto source, Piece destination, ResolutionContext context)
        {
            return source.FENChar switch
            {
                FENChar.WhitePawn => new Pawn(source.Color),
                FENChar.WhiteKnight => new Knight(source.Color),
                FENChar.WhiteBishop => new Bishop(source.Color),
                FENChar.WhiteRook => new Rook(source.Color),
                FENChar.WhiteQueen => new Queen(source.Color),
                FENChar.WhiteKing => new King(source.Color),
                FENChar.BlackPawn => new Pawn(source.Color),
                FENChar.BlackKnight => new Knight(source.Color),
                FENChar.BlackBishop => new Bishop(source.Color),
                FENChar.BlackRook => new Rook(source.Color),
                FENChar.BlackQueen => new Queen(source.Color),
                FENChar.BlackKing => new King(source.Color),
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, "Cannot convert to Piece")
            };
        }
    }
}