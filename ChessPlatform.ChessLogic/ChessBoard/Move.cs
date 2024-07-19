using ChessPlatform.Models.Chess;
using ChessPlatform.Models.Chess.Pieces;

namespace ChessPlatform.ChessLogic.ChessBoard;

public partial class ChessBoard
{
    public void Move(Coords previousCoords, Coords newCoords, FENChar? promotedType = null)
    {
        if (IsOver)
            throw new InvalidOperationException("Game is over");
        
        if (!AreCoordsValid(previousCoords) || !AreCoordsValid(newCoords))
            return;
        
        var piece = Board[previousCoords.Row, previousCoords.Column];
        
        if (piece is null || piece.Color != PlayerTurn)
            return;
        
        var pieceSafeSquares = SafeSquares[previousCoords];
        
        if (pieceSafeSquares.All(coords => coords != newCoords))
            throw new InvalidOperationException("Invalid move");

        switch (piece)
        {
            case King:
                switch (piece.Color)
                {
                    case Color.White:
                        CanWhiteCastleKingSide = false;
                        CanWhiteCastleQueenSide = false;
                        break;
                    case Color.Black:
                        CanBlackCastleKingSide = false;
                        CanBlackCastleQueenSide = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            case Rook:
                switch (piece.Color)
                {
                    case Color.White:
                        if (previousCoords.Row == 0)
                        {
                            switch (previousCoords.Column)
                            {
                                case 0:
                                    CanWhiteCastleQueenSide = false;
                                    break;
                                case 7:
                                    CanWhiteCastleKingSide = false;
                                    break;
                            }
                        }
                        break;
                    case Color.Black:
                        if (previousCoords.Row == 7)
                        {
                            switch (previousCoords.Column)
                            {
                                case 0:
                                    CanBlackCastleQueenSide = false;
                                    break;
                                case 7:
                                    CanBlackCastleKingSide = false;
                                    break;
                            }
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            case Pawn pawn:
                pawn.SetHasMoved();
                break;
        }
        
        var isPieceCaptured = Board[newCoords.Row, newCoords.Column] is not null;

        if (isPieceCaptured || piece is Pawn)
            _fiftyMoveRuleCounter = 0;
        else
            _fiftyMoveRuleCounter += 0.5f;
        
        Board[newCoords.Row, newCoords.Column] = piece switch
        {
            Pawn when newCoords.Row is 0 or 7 => PromotedPiece(promotedType),
            _ => piece
        };
        
        Board[previousCoords.Row, previousCoords.Column] = null;
        HandleSpecialMoves(piece, previousCoords, newCoords);
        
        var lastMovePlayedAt = LastMove?.PlayedAt ?? CreatedAt;
        var timeSinceLastMove = DateTime.UtcNow - lastMovePlayedAt;
        
        var increment = TimeSpan.FromSeconds(int.Parse(TimeControl.Split('+').Last()));
        
        switch (PlayerTurn)
        {
            case Color.White:
                WhitePlayerRemainingTime -= timeSinceLastMove - increment;
                break;
            case Color.Black:
                BlackPlayerRemainingTime -= timeSinceLastMove - increment;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        LastMove = new LastMove(piece, previousCoords, newCoords, DateTime.UtcNow);
        PlayerTurn = PlayerTurn == Color.White ? Color.Black : Color.White;
        IsInCheck(PlayerTurn, true);
        SafeSquares = FindSafeSquares();
        
        if (PlayerTurn == Color.White)
            _fullMoveCounter++;
        
        BoardAsFen = _fenConverter.ConvertBoardToFen(Board, PlayerTurn, LastMove, _fiftyMoveRuleCounter,
            _fullMoveCounter, CanWhiteCastleKingSide, CanWhiteCastleQueenSide, CanBlackCastleKingSide,
            CanBlackCastleQueenSide);
        
        UpdateThreeFoldRepetitionDictionary(BoardAsFen);
        
        IsOver = IsGameFinished();
    }
}
