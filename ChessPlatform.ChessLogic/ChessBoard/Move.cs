using ChessPlatform.Models.Chess;
using ChessPlatform.Models.Chess.Pieces;

namespace ChessPlatform.ChessLogic.ChessBoard;

public partial class ChessBoard
{
    public void Move(Coords previousCoords, Coords newCoords, FENChar? promotedType = null)
    {
        if (!AreCoordsValid(previousCoords) || !AreCoordsValid(newCoords))
            return;
        
        var piece = Board[previousCoords.Row, previousCoords.Column];
        
        if (piece is null || piece.Color != PlayerColor)
            return;
        
        var pieceSafeSquares = SafeSquares[previousCoords];
        
        if (pieceSafeSquares.All(coords => coords != newCoords))
            throw new InvalidOperationException("Invalid move");

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

        Board[newCoords.Row, newCoords.Column] = promotedType switch
        {
            null => piece,
            _ => PromotedPiece(promotedType.Value)
        };
        
        Board[previousCoords.Row, previousCoords.Column] = null;
        HandleSpecialMoves(piece, previousCoords, newCoords);
        
        LastMove = new LastMove(piece, previousCoords, newCoords);
        PlayerColor = PlayerColor == Color.White ? Color.Black : Color.White;
        IsInCheck(PlayerColor, true);
        SafeSquares = FindSafeSquares();
    }
}
