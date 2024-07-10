using ChessPlatform.Models.Chess;
using ChessPlatform.Models.Chess.Pieces;

namespace ChessPlatform.ChessLogic.ChessBoard;

public partial class ChessBoard
{
    private Piece PromotedPiece(FENChar promotedPieceType)
    {
        return promotedPieceType switch
        {
            FENChar.WhiteKnight or FENChar.BlackKnight => new Knight(PlayerColor),
            FENChar.WhiteBishop or FENChar.BlackBishop => new Bishop(PlayerColor),
            FENChar.WhiteRook or FENChar.BlackRook => new Rook(PlayerColor),
            FENChar.WhiteQueen or FENChar.BlackQueen => new Queen(PlayerColor),
            _ => throw new ArgumentOutOfRangeException(nameof(promotedPieceType), promotedPieceType,
                "This piece can't be promoted")
        };
    }
}
