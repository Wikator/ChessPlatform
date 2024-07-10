using ChessPlatform.Models.Chess;
using ChessPlatform.Models.Chess.Pieces;

namespace ChessPlatform.ChessLogic.ChessBoard;

public partial class ChessBoard
{
    private bool IsInCheck(Color playerColor, bool currentPosition)
    {
        for (var row = 0; row < BoardSize; row++)
        {
            for (var column = 0; column < BoardSize; column++)
            {
                var piece = Board[row, column];
                
                if (piece is null || piece.Color == playerColor)
                    continue;

                foreach (var (directionRow, directionColumn) in piece.Directions)
                {
                    var newCoords = new Coords(row + directionRow, column + directionColumn);
                    
                    if (!AreCoordsValid(newCoords))
                        continue;

                    if (piece is Pawn or Knight or King)
                    {
                        if (piece is Pawn && directionColumn == 0)
                            continue;
                        
                        var attackedPiece = Board[newCoords.Row, newCoords.Column];

                        if (attackedPiece is not King || attackedPiece.Color != playerColor)
                            continue;
                        
                        if (currentPosition)
                            CheckedKingCoords = newCoords;
                        
                        return true;
                    }

                    while (AreCoordsValid(newCoords))
                    {
                        var attackedPiece = Board[newCoords.Row, newCoords.Column];

                        if (attackedPiece is King && attackedPiece.Color == playerColor)
                        {
                            if (currentPosition)
                                CheckedKingCoords = newCoords;
                                
                            return true;
                        }

                        if (attackedPiece is not null)
                            break;
                            
                        newCoords = new Coords(newCoords.Row + directionRow, newCoords.Column + directionColumn);
                    }
                }
            }
        }

        if (currentPosition)
            CheckedKingCoords = null;
        
        return false;
    }
}
