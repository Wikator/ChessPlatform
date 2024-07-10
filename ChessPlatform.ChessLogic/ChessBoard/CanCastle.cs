using ChessPlatform.Models.Chess;
using ChessPlatform.Models.Chess.Pieces;

namespace ChessPlatform.ChessLogic.ChessBoard;

public partial class ChessBoard
{
    private bool CanCastle(King king, bool kingSideCastle)
    {
        if (CheckedKingCoords is not null || king.HasMoved)
            return false;

        var row = king.Color == Color.White ? 0 : 7;
        var kingCoords = new Coords(row, 4);
        var rookCoords = new Coords(row, kingSideCastle ? 7 : 0);
        
        if (Board[rookCoords.Row, rookCoords.Column] is not Rook rook || rook.HasMoved)
            return false;
        
        var firstNextKingPositionColumn = kingSideCastle ? 5 : 3;
        var secondNextKingPositionColumn = kingSideCastle ? 6 : 2;
        
        if (Board[kingCoords.Row, firstNextKingPositionColumn] is not null
            || Board[kingCoords.Row, secondNextKingPositionColumn] is not null)
            return false;
        
        if (!kingSideCastle && Board[kingCoords.Row, 1] is not null)
            return false;
        
        return IsPositionSafeAfterMove(kingCoords, new Coords(row, firstNextKingPositionColumn))
               && IsPositionSafeAfterMove(kingCoords, new Coords(row, secondNextKingPositionColumn));
    }
}
