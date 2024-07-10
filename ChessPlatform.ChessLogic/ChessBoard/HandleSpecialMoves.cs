using ChessPlatform.Models.Chess;
using ChessPlatform.Models.Chess.Pieces;

namespace ChessPlatform.ChessLogic.ChessBoard;

public partial class ChessBoard
{
    private void HandleSpecialMoves(Piece piece, Coords previousCoords, Coords newCoords)
    {
        
        switch (piece)
        {
            case King when Math.Abs(newCoords.Column - previousCoords.Column) == 2:
                var rookPositionRow = previousCoords.Row;
                var rookPositionColumn = newCoords.Column > previousCoords.Column ? 7 : 0;

                if (Board[rookPositionRow, rookPositionColumn] is Rook rook)
                {
                    var rookNewPositionY = newCoords.Column > previousCoords.Column ? 5 : 3;
                    Board[rookPositionRow, rookPositionColumn] = null;
                    Board[rookPositionRow, rookNewPositionY] = rook;
                    rook.SetHasMoved();
                }
                break;
            case Pawn when LastMove?.Piece is Pawn
                           && Math.Abs(LastMove.Value.To.Row - LastMove.Value.From.Row) == 2
                           && previousCoords.Row == LastMove.Value.To.Row
                           && newCoords.Column == LastMove.Value.To.Column:
                Board[LastMove.Value.To.Row, LastMove.Value.To.Column] = null;
                break;
        }
    }
}
