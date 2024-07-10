using ChessPlatform.Models.Chess;
using ChessPlatform.Models.Chess.Pieces;

namespace ChessPlatform.ChessLogic.ChessBoard;

public partial class ChessBoard
{
    private bool CanCaptureEnPassant(Pawn pawn, Coords pawnCoords)
    {
        if (LastMove is null)
            return false;
        
        var (piece, previousCoords, currentCoords) = LastMove.Value;

        if (piece is not Pawn
            || pawn.Color != PlayerColor
            || Math.Abs(currentCoords.Row - previousCoords.Row) != 2
            || pawnCoords.Row != currentCoords.Row
            || Math.Abs(pawnCoords.Column - currentCoords.Column) != 1)
            return false;
        
        var pawnNewPositionRow = pawnCoords.Row + (pawn.Color == Color.White ? 1 : -1);
        var pawnNewPositionColumn = currentCoords.Column;
        
        Board[currentCoords.Row, currentCoords.Column] = null;
        Board[pawnNewPositionRow, pawnNewPositionColumn] = pawn;
        
        var isSafe = !IsInCheck(pawn.Color, false);
        
        Board[currentCoords.Row, currentCoords.Column] = piece;
        Board[pawnNewPositionRow, pawnNewPositionColumn] = null;
        
        return isSafe;
    }
}
