using ChessPlatform.Models.Chess;

namespace ChessPlatform.ChessLogic.ChessBoard;

public partial class ChessBoard
{
    private bool IsPositionSafeAfterMove(Coords previousCoords, Coords newCoords)
    {
        var piece = Board[previousCoords.Row, previousCoords.Column];
        
        if (piece is null)
            throw new InvalidOperationException("Invalid move");
        
        var newPiece = Board[newCoords.Row, newCoords.Column];
        
        if (newPiece is not null && newPiece.Color == piece.Color)
            return false;
        
        Board[previousCoords.Row, previousCoords.Column] = null;
        Board[newCoords.Row, newCoords.Column] = piece;
        
        var isSafe = !IsInCheck(piece.Color, false);
        
        Board[previousCoords.Row, previousCoords.Column] = piece;
        Board[newCoords.Row, newCoords.Column] = newPiece;
        
        return isSafe;
    }
}
