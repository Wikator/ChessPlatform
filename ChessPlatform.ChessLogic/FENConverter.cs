using System.Text;
using ChessPlatform.ChessLogic.Extensions;
using ChessPlatform.Models.Chess;
using ChessPlatform.Models.Chess.Pieces;

namespace ChessPlatform.ChessLogic;

public class FenConverter
{
    private readonly IReadOnlyList<char> _columns = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h']; 
    
    public string ConvertBoardToFen(Piece?[,] board, Color playerColor, LastMove? lastMove,
        float fiftyMoveRuleCounter, ushort numberOfFullMoves, bool canWhiteCastleKingSide, bool canWhiteCastleQueenSide,
        bool canBlackCastleKingSide, bool canBlackCastleQueenSide)
    {
        var fen = new StringBuilder();

        for (var row = 7; row >= 0; row--)
        {
            var fenRow = new StringBuilder();
            var consecutiveEmptySquaresCounter = 0;
            
            for (var column = 0; column < 8; column++)
            {
                var piece = board[row, column];

                if (piece is null)
                {
                    consecutiveEmptySquaresCounter++;
                    continue;
                }

                if (consecutiveEmptySquaresCounter > 0)
                    fenRow.Append(consecutiveEmptySquaresCounter);
                
                consecutiveEmptySquaresCounter = 0;
                fenRow.Append(piece.FENChar.ToFenString());
            }
            
            if (consecutiveEmptySquaresCounter > 0)
                fenRow.Append(consecutiveEmptySquaresCounter);
            
            fen.Append(row == 0 ? fenRow : $"{fenRow}/");
        }
        
        var playerColorChar = playerColor == Color.White ? 'w' : 'b';
        fen.Append($" {playerColorChar}");
        fen.Append($" {CastlingAvailability(canWhiteCastleKingSide, canWhiteCastleQueenSide,
            canBlackCastleKingSide, canBlackCastleQueenSide)}");
        fen.Append($" {EnPassantPossibility(lastMove, playerColor)}");
        fen.Append($" {fiftyMoveRuleCounter * 2}");
        fen.Append($" {numberOfFullMoves}");
        return fen.ToString();
    }
    
    public static Piece?[,] ConvertFenToBoard(string fen)
    {
        var pieces = new Piece?[8, 8];
        var splitFen = fen.Split(' ');
        var boardFen = splitFen[0];
        var pieceCounter = 0;

        boardFen = string.Join("", boardFen.Reverse());
        Console.WriteLine(boardFen);
        boardFen = string.Join("", boardFen.Split("/").Select(a => string.Join("", a.Reverse())));
        
        Console.WriteLine(boardFen.ToString());
        

        foreach (var character in boardFen)
        {
            if (character == '/')
                continue;
            
            if (char.IsDigit(character))
            {
                pieceCounter += int.Parse(character.ToString());
                continue;
            }
            
            var row = pieceCounter / 8;
            var column = pieceCounter % 8;
            
            pieces[row, column] = (FENChar)character switch
            {
                FENChar.WhitePawn => new Pawn(Color.White),
                FENChar.WhiteKnight => new Knight(Color.White),
                FENChar.WhiteBishop => new Bishop(Color.White),
                FENChar.WhiteRook => new Rook(Color.White),
                FENChar.WhiteQueen => new Queen(Color.White),
                FENChar.WhiteKing => new King(Color.White),
                FENChar.BlackPawn => new Pawn(Color.Black),
                FENChar.BlackKnight => new Knight(Color.Black),
                FENChar.BlackBishop => new Bishop(Color.Black),
                FENChar.BlackRook => new Rook(Color.Black),
                FENChar.BlackQueen => new Queen(Color.Black),
                FENChar.BlackKing => new King(Color.Black),
                _ => null
            };
            
            if (pieces[row, column] is Pawn pawn &&
                (pawn.Color == Color.White && row != 1 || pawn.Color == Color.Black && row != 6))
                pawn.SetHasMoved();
            
            pieceCounter++;
        }
        
        return pieces;
    }

    private static string CastlingAvailability(bool canWhiteCastleKingSide, bool canWhiteCastleQueenSide,
        bool canBlackCastleKingSide, bool canBlackCastleQueenSide)
    {
        var castlingAvailabilityBuilder = new StringBuilder();
        
        if (canWhiteCastleKingSide)
            castlingAvailabilityBuilder.Append('K');
        
        if (canWhiteCastleQueenSide)
            castlingAvailabilityBuilder.Append('Q');
        
        if (canBlackCastleKingSide)
            castlingAvailabilityBuilder.Append('k');
        
        if (canBlackCastleQueenSide)
            castlingAvailabilityBuilder.Append('q');
        
        var castlingAvailability = castlingAvailabilityBuilder.ToString();
        
        return string.IsNullOrEmpty(castlingAvailability) ? "-" : castlingAvailability;
    }

    private string EnPassantPossibility(LastMove? lastMove, Color color)
    {
        if (lastMove?.Piece is not Pawn || Math.Abs(lastMove.Value.To.Row - lastMove.Value.From.Row) != 2)
            return "-";
        
        var row = color == Color.White ? 6 : 3;
        return _columns[lastMove.Value.From.Column] + row.ToString();
    }
}
