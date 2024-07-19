using ChessPlatform.Models.Chess;

namespace ChessPlatform.ChessLogic.ChessBoard;

public partial class ChessBoard
{
    private bool IsGameFinished()
    {
        if (_threeFoldRepetitionFlag)
        {
            GameOverStatus = "Draw by threefold repetition";
            IsOver = true;
            return true;
        }
        
        if (InsufficientMaterial())
        {
            GameOverStatus = "Draw by insufficient material";
            IsOver = true;
            return true;
        }
        
        if (_fiftyMoveRuleCounter >= 50)
        {
            GameOverStatus = "Draw by fifty-move rule";
            IsOver = true;
            return true;
        }
        
        if (SafeSquares.Any(coords => coords.Value.Any()))
            return false;
        
        IsOver = true;
        
        if (CheckedKingCoords is not null)
        {
            var previousPlayerColor = PlayerTurn == Color.White ? "Black" : "White";
            GameOverStatus = $"{previousPlayerColor} won by checkmate";
        }
        else
        {
            GameOverStatus = "Draw by stalemate";
        }
            
        return true;
    }
}