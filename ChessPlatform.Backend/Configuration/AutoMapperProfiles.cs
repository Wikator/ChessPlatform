using AutoMapper;
using ChessPlatform.ChessLogic;
using ChessPlatform.ChessLogic.Models;
using ChessPlatform.ChessLogic.Pieces;
using ChessPlatform.Shared;

namespace ChessPlatform.Backend.Configuration;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Piece, PieceDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.GetType().Name))
            .ForMember(dest => dest.HasMoved, opt => opt.MapFrom(src => GetPieceHasMoved(src)));

        CreateMap<ChessBoard, GameDto>()
            .ForMember(dest => dest.Board, opt => opt.MapFrom(src => Convert2DArrayToJaggedArray(src.Board)));


        CreateMap<Coords, CoordsDto>();
        CreateMap<LastMove, LastMoveDto>();
    }

    private static T[][] Convert2DArrayToJaggedArray<T>(T[,] array2D)
    {
        var jaggedArray = new T[8][];
        for (var i = 0; i < 8; i++)
        {
            jaggedArray[i] = new T[8];
            for (var j = 0; j < 8; j++)
            {
                jaggedArray[i][j] = array2D[i, j];
            }
        }
                
        return jaggedArray;
    }
    
    private static bool? GetPieceHasMoved(Piece? piece) => piece switch
    {
        King king => king.HasMoved,
        Rook rook => rook.HasMoved,
        Pawn pawn => pawn.HasMoved,
        _ => null
    };
}