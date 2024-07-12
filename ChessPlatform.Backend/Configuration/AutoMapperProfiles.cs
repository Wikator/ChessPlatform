using AutoMapper;
using ChessPlatform.Backend.Entities;
using ChessPlatform.ChessLogic.ChessBoard;
using ChessPlatform.Models.Chess;
using ChessPlatform.Models.Chess.Pieces;
using ChessPlatform.Models.DTOs;

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
        
        CreateMap<ChessBoard, ChessBoardEntity>()
            .ConvertUsing<ChessBoardConverter>();
        
        CreateMap<ChessGameEntity, ChessBoard>()
            .ConvertUsing<ChessGameEntityConverter>();
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
    
    // ReSharper disable once ClassNeverInstantiated.Local
    private class ChessBoardConverter : ITypeConverter<ChessBoard, ChessBoardEntity>
    {
        public ChessBoardEntity Convert(ChessBoard source, ChessBoardEntity destination, ResolutionContext context)
        {
            return new ChessBoardEntity
            {
                Pieces = GetPieceEntities(source.Board),
                LastMove = GetLastMoveEntity(source.LastMove),
                PlayerTurn = source.PlayerColor
            };
        }

        #region PrivateMethods
        
        private static List<PieceEntity> GetPieceEntities(Piece?[,] pieces)
        {
            var pieceEntities = new List<PieceEntity>();
            
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    var piece = pieces[i, j];
                    
                    if (piece is null)
                        continue;
                    
                    var pieceEntity = new PieceEntity
                    {
                        Type = piece.GetType().Name,
                        Color = piece.Color,
                        HasMoved = GetPieceHasMoved(piece),
                        Row = i,
                        Column = j
                    };
                    
                    pieceEntities.Add(pieceEntity);
                }
            }
            
            return pieceEntities;
        }
        
        private static LastMoveEntity? GetLastMoveEntity(LastMove? lastMove)
        {
            if (lastMove is null)
                return null;
            
            return new LastMoveEntity
            {
                FromRow = lastMove.Value.From.Row,
                FromColumn = lastMove.Value.From.Column,
                ToRow = lastMove.Value.To.Row,
                ToColumn = lastMove.Value.To.Column
            };
        }
        
        #endregion
    }
    
    // ReSharper disable once ClassNeverInstantiated.Local
    private class ChessGameEntityConverter : ITypeConverter<ChessGameEntity, ChessBoard>
    {
        public ChessBoard Convert(ChessGameEntity source, ChessBoard destination, ResolutionContext context)
        {
            var pieces = GetPieces2DArray(source.ChessBoard!.Pieces);

            var lastMove = GetLastMove(source.ChessBoard.LastMove, pieces);
            
            var game = new ChessBoard(pieces, source.ChessBoard.PlayerTurn, lastMove)
            {
                WhitePlayerId = source.WhitePlayerId,
                BlackPlayerId = source.BlackPlayerId
            };
            return game;
        }

        #region PrivateMethods

        private static Piece?[,] GetPieces2DArray(IEnumerable<PieceEntity> pieceEntities)
        {
            var pieces = new Piece?[8, 8];
            
            foreach (var pieceEntity in pieceEntities)
            {
                Piece piece = pieceEntity.Type switch
                {
                    nameof(Pawn) => new Pawn(pieceEntity.Color),
                    nameof(Knight) => new Knight(pieceEntity.Color),
                    nameof(Bishop) => new Bishop(pieceEntity.Color),
                    nameof(Rook) => new Rook(pieceEntity.Color),
                    nameof(Queen) => new Queen(pieceEntity.Color),
                    nameof(King) => new King(pieceEntity.Color),
                    _ => throw new ArgumentOutOfRangeException(nameof(pieceEntity.Type), pieceEntity.Type, "Cannot convert to Piece")
                };
                
                if (pieceEntity.HasMoved is not null && pieceEntity.HasMoved.Value)
                {
                    switch (piece)
                    {
                        case King king:
                            king.SetHasMoved();
                            break;
                        case Rook rook:
                            rook.SetHasMoved();
                            break;
                        case Pawn pawn:
                            pawn.SetHasMoved();
                            break;
                    }
                }
                
                pieces[pieceEntity.Row, pieceEntity.Column] = piece;
            }
            
            return pieces;
        }
        
        private static LastMove? GetLastMove(LastMoveEntity? lastMoveEntity, Piece?[,] pieces)
        {
            if (lastMoveEntity is null)
                return null;
            
            var from = new Coords(lastMoveEntity.FromRow, lastMoveEntity.FromColumn);
            var to = new Coords(lastMoveEntity.ToRow, lastMoveEntity.ToColumn);
            
            return new LastMove(pieces[from.Row, from.Column]!, from, to);
        }

        #endregion
    }
}