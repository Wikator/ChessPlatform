using ChessPlatform.Models.Chess;
using ChessPlatform.Models.Chess.Pieces;

namespace ChessPlatform.ChessLogic.ChessBoard;

public partial class ChessBoard
{
    private bool InsufficientMaterial()
    {
        var whitePieces = new List<PieceWithCoords>();
        var blackPieces = new List<PieceWithCoords>();
        
        for (var row = 0; row < BoardSize; row++)
        {
            for (var column = 0; column < BoardSize; column++)
            {
                var piece = Board[row, column];
                
                if (piece is null)
                    continue;
                
                if (piece.Color == Color.White)
                    whitePieces.Add(new PieceWithCoords(piece, new Coords(row, column)));
                else
                    blackPieces.Add(new PieceWithCoords(piece, new Coords(row, column)));
            }
        }
        
        // King vs King
        if (whitePieces.Count == 1 && blackPieces.Count == 1)
            return true;
        
        // King and Minor Piece vs King
        if (whitePieces.Count == 2 && blackPieces.Count == 1)
            return whitePieces.Any(tuple => tuple.Piece is Bishop or Knight);
        
        if (whitePieces.Count == 1 && blackPieces.Count == 2)
            return blackPieces.Any(tuple => tuple.Piece is Bishop or Knight);
        
        // King and Bishop vs King and Bishop with the bishops on the same color
        
        if (whitePieces.Count == 2 && blackPieces.Count == 2)
        {
            var whiteBishop = whitePieces.FirstOrDefault(tuple => tuple.Piece is Bishop);
            var blackBishop = blackPieces.FirstOrDefault(tuple => tuple.Piece is Bishop);

            if (whiteBishop is not null && blackBishop is not null)
            {
                var whiteBishopColor = (whiteBishop.Coords.Row + whiteBishop.Coords.Column) % 2;
                var blackBishopColor = (blackBishop.Coords.Row + blackBishop.Coords.Column) % 2;
            
                return whiteBishopColor == blackBishopColor;
            }
        }

        if (blackPieces.Count == 1 && PlayerHasOnlyKingAndTwoKnights(whitePieces))
            return true;
        
        if (whitePieces.Count == 1 && PlayerHasOnlyKingAndTwoKnights(blackPieces))
            return true;

        if (whitePieces.Count >= 3 && blackPieces.Count == 1 && PlayerHasOnlyBishopsOfTheSameColorAgainstKing(whitePieces))
            return true;
        
        if (blackPieces.Count >= 3 && whitePieces.Count == 1 && PlayerHasOnlyBishopsOfTheSameColorAgainstKing(blackPieces))
            return true;
        
        return false;
        
        bool PlayerHasOnlyKingAndTwoKnights(IReadOnlyCollection<PieceWithCoords> pieces)
        {
            return pieces.Count == 3 && pieces.All(tuple => tuple.Piece is Knight or King);
        }

        bool PlayerHasOnlyBishopsOfTheSameColorAgainstKing(IReadOnlyCollection<PieceWithCoords> pieces)
        {
            var bishops = pieces.Where(tuple => tuple.Piece is Bishop).ToList();
            var areAllBishopsSameColor = bishops.All(piece => (piece.Coords.Row + piece.Coords.Column) % 2 == 0);
            return bishops.Count == pieces.Count - 1 && areAllBishopsSameColor;
        }
    }
    
    private record PieceWithCoords(Piece Piece, Coords Coords);
}
