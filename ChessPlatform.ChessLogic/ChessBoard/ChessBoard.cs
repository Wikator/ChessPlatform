using ChessPlatform.Models.Chess;
using ChessPlatform.Models.Chess.Pieces;

namespace ChessPlatform.ChessLogic.ChessBoard;

public partial class ChessBoard
{
    private const int BoardSize = 8;
    public Dictionary<Coords, IEnumerable<Coords>> SafeSquares { get; private set; }
    public LastMove? LastMove { get; private set; }
    public Coords? CheckedKingCoords { get; private set; }
    
    public string? WhitePlayerId { get; set; }
    public string? BlackPlayerId { get; set; }
    
    public bool IsOver { get; private set; }
    public string? GameOverStatus { get; private set; }
    private float _fiftyMoveRuleCounter;
    private ushort _fullMoveCounter;
    private Dictionary<string, ushort> _threeFoldRepetitionDictionary = new();
    private bool _threeFoldRepetitionFlag;
    public string BoardAsFen { get; private set; } = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    private readonly FenConverter _fenConverter = new();
    
    public TimeSpan WhitePlayerRemainingTime { get; set; }
    public TimeSpan BlackPlayerRemainingTime { get; set; }
    
    public string TimeControl { get; init; } = "1+5";
    
    public bool CanWhiteCastleKingSide { get; private set; } = true;
    public bool CanWhiteCastleQueenSide { get; private set; } = true;
    public bool CanBlackCastleKingSide { get; private set; } = true;
    public bool CanBlackCastleQueenSide { get; private set; } = true;
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    
    public ChessBoard()
    {
        SafeSquares = FindSafeSquares();
    }
    
    public ChessBoard(Piece?[][] board, Color playerTurn)
    {
        Board = new Piece?[BoardSize, BoardSize];
        
        for (var row = 0; row < BoardSize; row++)
        {
            for (var column = 0; column < BoardSize; column++)
            {
                Board[row, column] = board[row][column];
            }
        }
        
        PlayerTurn = playerTurn;
        SafeSquares = FindSafeSquares();
    }
    
    public ChessBoard(Piece?[,] board, Color playerTurn, LastMove? lastMove)
    {
        Board = board;
        LastMove = lastMove;
        
        PlayerTurn = playerTurn;
        SafeSquares = FindSafeSquares();
    }
    
    public ChessBoard(Piece?[,] board, Color playerTurn, LastMove? lastMove, string timeControl, bool isOver,
        TimeSpan whitePlayerRemainingTime, TimeSpan blackPlayerRemainingTime,bool canWhiteCastleKingSide,
        bool canWhiteCastleQueenSide, bool canBlackCastleKingSide, bool canBlackCastleQueenSide)
    {
        Board = board;
        LastMove = lastMove;
        
        PlayerTurn = playerTurn;
        SafeSquares = FindSafeSquares();
        
        WhitePlayerRemainingTime = whitePlayerRemainingTime;
        BlackPlayerRemainingTime = blackPlayerRemainingTime;
        
        TimeControl = timeControl;
        IsOver = isOver;
        
        CanWhiteCastleKingSide = canWhiteCastleKingSide;
        CanWhiteCastleQueenSide = canWhiteCastleQueenSide;
        CanBlackCastleKingSide = canBlackCastleKingSide;
        CanBlackCastleQueenSide = canBlackCastleQueenSide;
    }

    public Piece?[,] Board { get; } =
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

    public Color PlayerTurn { get; private set; } = Color.White;
    
    private static bool AreCoordsValid(Coords coords) =>
        coords.Row is >= 0 and < BoardSize && coords.Column is >= 0 and < BoardSize;

    public Piece? this[int row, int column] => Board[row, column];
    public Piece? this[Coords coords] => Board[coords.Row, coords.Column];
    
    private void UpdateThreeFoldRepetitionDictionary(string fen)
    {
        var threeFoldRepetitionFenKey = string.Join("", fen.Split(' ').Take(4));

        if (_threeFoldRepetitionDictionary.TryGetValue(threeFoldRepetitionFenKey, out var value))
        {
            if (value == 2)
            {
                _threeFoldRepetitionFlag = true;
                return;
            }

            _threeFoldRepetitionDictionary[threeFoldRepetitionFenKey] = 2;
        }
        else
        {
            _threeFoldRepetitionDictionary[threeFoldRepetitionFenKey] = 1;
        }
    }
}
