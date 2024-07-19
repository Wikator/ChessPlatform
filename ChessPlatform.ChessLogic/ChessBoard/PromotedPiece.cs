using ChessPlatform.Models.Chess;
using ChessPlatform.Models.Chess.Pieces;

namespace ChessPlatform.ChessLogic.ChessBoard;

public partial class ChessBoard
{
    private Piece PromotedPiece(FENChar? promotedPieceType)
    {
        return promotedPieceType switch
        {
            FENChar.WhiteKnight or FENChar.BlackKnight => new Knight(PlayerTurn),
            FENChar.WhiteBishop or FENChar.BlackBishop => new Bishop(PlayerTurn),
            FENChar.WhiteRook or FENChar.BlackRook => new Rook(PlayerTurn),
            FENChar.WhiteQueen or FENChar.BlackQueen or null => new Queen(PlayerTurn),
            _ => throw new ArgumentOutOfRangeException(nameof(promotedPieceType), promotedPieceType,
                "This piece can't be promoted")
        };
    }
}
