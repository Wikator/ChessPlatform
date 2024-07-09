using ChessPlatform.ChessLogic.Models;
using ChessPlatform.ChessLogic.Pieces;

namespace ChessPlatform.ChessLogic;

public class ChessBoard
{
    private const int BoardSize = 8;
    public Dictionary<Coords, Coords[]> SafeSquares { get; private set; }
    public LastMove? LastMove { get; private set; }
    public Coords? CheckedKingCoords { get; private set; }
    
    public string? WhitePlayerId { get; set; }
    public string? BlackPlayerId { get; set; }
    
    public ChessBoard()
    {
        SafeSquares = FindSafeSquares();
    }
    
    public ChessBoard(Piece?[][] board, Color playerColor)
    {
        _board = new Piece?[BoardSize, BoardSize];
        
        for (var row = 0; row < BoardSize; row++)
        {
            for (var column = 0; column < BoardSize; column++)
            {
                _board[row, column] = board[row][column];
            }
        }
        
        PlayerColor = playerColor;
        SafeSquares = FindSafeSquares();
    }
    
    public ChessBoard(Piece?[,] board, Color playerColor, LastMove? lastMove)
    {
        _board = board;
        LastMove = lastMove;
        
        PlayerColor = playerColor;
        SafeSquares = FindSafeSquares();
    }
    
    private readonly Piece?[,] _board =
    {
        {
            new Rook(Color.White), new Knight(Color.White), new Bishop(Color.White), new Queen(Color.White),
            new King(Color.White), new Bishop(Color.White), new Knight(Color.White), new Rook(Color.White)
        },
        {
            new Pawn(Color.White), new Pawn(Color.White), new Pawn(Color.White), new Pawn(Color.White),
            new Pawn(Color.White), new Pawn(Color.White), new Pawn(Color.White), new Pawn(Color.White)
        },
        {null, null, null, null, null, null, null, null},
        {null, null, null, null, null, null, null, null},
        {null, null, null, null, null, null, null, null},
        {null, null, null, null, null, null, null, null},
        {
            new Pawn(Color.Black), new Pawn(Color.Black), new Pawn(Color.Black), new Pawn(Color.Black),
            new Pawn(Color.Black), new Pawn(Color.Black), new Pawn(Color.Black), new Pawn(Color.Black)
        },
        {
            new Rook(Color.Black), new Knight(Color.Black), new Bishop(Color.Black), new Queen(Color.Black),
            new King(Color.Black), new Bishop(Color.Black), new Knight(Color.Black), new Rook(Color.Black)
        }
    };
    
    public Piece?[,] Board => _board;

    public Color PlayerColor { get; private set; } = Color.White;

    private bool IsInCheck(Color playerColor, bool currentPosition)
    {
        for (var row = 0; row < BoardSize; row++)
        {
            for (var column = 0; column < BoardSize; column++)
            {
                var piece = _board[row, column];
                
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
                        
                        var attackedPiece = _board[newCoords.Row, newCoords.Column];

                        if (attackedPiece is not King || attackedPiece.Color != playerColor)
                            continue;
                        
                        if (currentPosition)
                            CheckedKingCoords = newCoords;
                        
                        return true;
                    }

                    while (AreCoordsValid(newCoords))
                    {
                        var attackedPiece = _board[newCoords.Row, newCoords.Column];

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
    
    private bool IsPositionSafeAfterMove(Coords previousCoords, Coords newCoords)
    {
        var piece = _board[previousCoords.Row, previousCoords.Column];
        
        if (piece is null)
            throw new InvalidOperationException("Invalid move");
        
        var newPiece = _board[newCoords.Row, newCoords.Column];
        
        if (newPiece is not null && newPiece.Color == piece.Color)
            return false;
        
        _board[previousCoords.Row, previousCoords.Column] = null;
        _board[newCoords.Row, newCoords.Column] = piece;
        
        var isSafe = !IsInCheck(piece.Color, false);
        
        _board[previousCoords.Row, previousCoords.Column] = piece;
        _board[newCoords.Row, newCoords.Column] = newPiece;
        
        return isSafe;
    }

    private Dictionary<Coords, Coords[]> FindSafeSquares()
    {
        var safeSquares = new Dictionary<Coords, Coords[]>();
        
        for (var row = 0; row < BoardSize; row++)
        {
            for (var column = 0; column < BoardSize; column++)
            {
                var currentCoords = new Coords(row, column);
                var piece = _board[currentCoords.Row, currentCoords.Column];
                
                if (piece is null || piece.Color != PlayerColor)
                    continue;

                var pieceSafeSquares = new List<Coords>();

                foreach (var (dx, dy) in piece.Directions)
                {
                    var newCoords = new Coords(row + dx, column + dy);
                    
                    if (!AreCoordsValid(newCoords))
                        continue;
                    
                    var newPiece = _board[newCoords.Row, newCoords.Column];
                    
                    if (newPiece is not null && newPiece.Color == piece.Color)
                        continue;

                    if (piece is Pawn)
                    {
                        switch (dx)
                        {
                            case 2 or -2 when newPiece is not null:
                            case 2 or -2 when _board[newCoords.Row + (dx == 2 ? -1 : 1), newCoords.Column] is not null:
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
                            newPiece = _board[newCoords.Row, newCoords.Column];
                            
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

                safeSquares[currentCoords] = pieceSafeSquares.ToArray();
            }
        }
        return safeSquares;
    }
    
    public void Move(Coords previousCoords, Coords newCoords, FENChar? promotedType = null)
    {
        if (!AreCoordsValid(previousCoords) || !AreCoordsValid(newCoords))
            return;
        
        var piece = _board[previousCoords.Row, previousCoords.Column];
        
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

        _board[newCoords.Row, newCoords.Column] = promotedType switch
        {
            null => piece,
            _ => PromotedPiece(promotedType.Value)
        };
        
        _board[previousCoords.Row, previousCoords.Column] = null;
        HandleSpecialMoves(piece, previousCoords, newCoords);
        
        LastMove = new LastMove(piece, previousCoords, newCoords);
        PlayerColor = PlayerColor == Color.White ? Color.Black : Color.White;
        IsInCheck(PlayerColor, true);
        SafeSquares = FindSafeSquares();
    }

    private bool CanCastle(King king, bool kingSideCastle)
    {
        if (CheckedKingCoords is not null || king.HasMoved)
            return false;

        var row = king.Color == Color.White ? 0 : 7;
        var kingCoords = new Coords(row, 4);
        var rookCoords = new Coords(row, kingSideCastle ? 7 : 0);
        
        if (_board[rookCoords.Row, rookCoords.Column] is not Rook rook || rook.HasMoved)
            return false;
        
        var firstNextKingPositionColumn = kingSideCastle ? 5 : 3;
        var secondNextKingPositionColumn = kingSideCastle ? 6 : 2;
        
        if (_board[kingCoords.Row, firstNextKingPositionColumn] is not null
            || _board[kingCoords.Row, secondNextKingPositionColumn] is not null)
            return false;
        
        if (!kingSideCastle && _board[kingCoords.Row, 1] is not null)
            return false;
        
        return IsPositionSafeAfterMove(kingCoords, new Coords(row, firstNextKingPositionColumn))
            && IsPositionSafeAfterMove(kingCoords, new Coords(row, secondNextKingPositionColumn));
    }

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
        
        _board[currentCoords.Row, currentCoords.Column] = null;
        _board[pawnNewPositionRow, pawnNewPositionColumn] = pawn;
        
        var isSafe = !IsInCheck(pawn.Color, false);
        
        _board[currentCoords.Row, currentCoords.Column] = piece;
        _board[pawnNewPositionRow, pawnNewPositionColumn] = null;
        
        return isSafe;
    }

    private void HandleSpecialMoves(Piece piece, Coords previousCoords, Coords newCoords)
    {
        
        switch (piece)
        {
            case King when Math.Abs(newCoords.Column - previousCoords.Column) == 2:
                var rookPositionRow = previousCoords.Row;
                var rookPositionColumn = newCoords.Column > previousCoords.Column ? 7 : 0;

                if (_board[rookPositionRow, rookPositionColumn] is Rook rook)
                {
                    var rookNewPositionY = newCoords.Column > previousCoords.Column ? 5 : 3;
                    _board[rookPositionRow, rookPositionColumn] = null;
                    _board[rookPositionRow, rookNewPositionY] = rook;
                    rook.SetHasMoved();
                }
                break;
            case Pawn when LastMove?.Piece is Pawn
                           && Math.Abs(LastMove.Value.To.Row - LastMove.Value.From.Row) == 2
                           && previousCoords.Row == LastMove.Value.To.Row
                           && newCoords.Column == LastMove.Value.To.Column:
                _board[LastMove.Value.To.Row, LastMove.Value.To.Column] = null;
                break;
        }
    }

    private Piece PromotedPiece(FENChar promotedPieceType)
    {
        return promotedPieceType switch
        {
            FENChar.WhiteKnight or FENChar.BlackKnight => new Knight(PlayerColor),
            FENChar.WhiteBishop or FENChar.BlackBishop => new Bishop(PlayerColor),
            FENChar.WhiteRook or FENChar.BlackRook => new Rook(PlayerColor),
            FENChar.WhiteQueen or FENChar.BlackQueen => new Queen(PlayerColor),
            _ => throw new ArgumentOutOfRangeException(nameof(promotedPieceType), promotedPieceType,
                "This piece can't be promoted")
        };
    }
    
    private static bool AreCoordsValid(Coords coords) =>
        coords.Row is >= 0 and < BoardSize && coords.Column is >= 0 and < BoardSize;

    public Piece? this[int row, int column] => _board[row, column];
    public Piece? this[Coords coords] => _board[coords.Row, coords.Column];
}