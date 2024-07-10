using ChessPlatform.Models.Chess;
using ChessPlatform.Models.Chess.Pieces;

namespace ChessPlatform.ChessLogic.ChessBoard;

public partial class ChessBoard
{
    private Dictionary<Coords, IEnumerable<Coords>> FindSafeSquares()
    {
        var safeSquares = new Dictionary<Coords, IEnumerable<Coords>>();
        
        for (var row = 0; row < BoardSize; row++)
        {
            for (var column = 0; column < BoardSize; column++)
            {
                var currentCoords = new Coords(row, column);
                var piece = Board[currentCoords.Row, currentCoords.Column];
                
                if (piece is null || piece.Color != PlayerColor)
                    continue;

                var pieceSafeSquares = new List<Coords>();

                foreach (var (dx, dy) in piece.Directions)
                {
                    var newCoords = new Coords(row + dx, column + dy);
                    
                    if (!AreCoordsValid(newCoords))
                        continue;
                    
                    var newPiece = Board[newCoords.Row, newCoords.Column];
                    
                    if (newPiece is not null && newPiece.Color == piece.Color)
                        continue;

                    if (piece is Pawn)
                    {
                        switch (dx)
                        {
                            case 2 or -2 when newPiece is not null:
                            case 2 or -2 when Board[newCoords.Row + (dx == 2 ? -1 : 1), newCoords.Column] is not null:
                            case 1 or -1 when dy is 0 && newPiece is not null:
                                continue;
                        }

                        if (dy is 1 or -1 && (newPiece is null || piece.Color == newPiece.Color))
                            continue;
                    }

                    if (piece is Pawn or Knight or King)
                    {
                        if (IsPositionSafeAfterMove(currentCoords, newCoords))
                            pieceSafeSquares.Add(newCoords);
                    }
                    else
                    {
                        while (AreCoordsValid(newCoords))
                        {
                            newPiece = Board[newCoords.Row, newCoords.Column];
                            
                            if (newPiece is not null && newPiece.Color == piece.Color)
                                break;
                            
                            if (IsPositionSafeAfterMove(currentCoords, newCoords))
                                pieceSafeSquares.Add(newCoords);
                            
                            if (newPiece is not null)
                                break;
                            
                            newCoords = new Coords(newCoords.Row + dx, newCoords.Column + dy);
                        }
                    }
                }
                
                switch (piece)
                {
                    case King king:
                    {
                        if (CanCastle(king, true))
                            pieceSafeSquares.Add(new Coords(row, 6));
                    
                        if (CanCastle(king, false))
                            pieceSafeSquares.Add(new Coords(row, 2));
                        break;
                    }
                    case Pawn pawn when CanCaptureEnPassant(pawn, currentCoords):
                        pieceSafeSquares.Add(new Coords(row + (pawn.Color == Color.White ? 1 : -1),
                            LastMove!.Value.From.Column));
                        break;
                }

                safeSquares[currentCoords] = pieceSafeSquares;
            }
        }
        return safeSquares;
    }
}
