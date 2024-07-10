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
    
    public ChessBoard()
    {
        SafeSquares = FindSafeSquares();
    }
    
    public ChessBoard(Piece?[][] board, Color playerColor)
    {
        Board = new Piece?[BoardSize, BoardSize];
        
        for (var row = 0; row < BoardSize; row++)
        {
            for (var column = 0; column < BoardSize; column++)
            {
                Board[row, column] = board[row][column];
            }
        }
        
        PlayerColor = playerColor;
        SafeSquares = FindSafeSquares();
    }
    
    public ChessBoard(Piece?[,] board, Color playerColor, LastMove? lastMove)
    {
        Board = board;
        LastMove = lastMove;
        
        PlayerColor = playerColor;
        SafeSquares = FindSafeSquares();
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

    public Color PlayerColor { get; private set; } = Color.White;
    
    private static bool AreCoordsValid(Coords coords) =>
        coords.Row is >= 0 and < BoardSize && coords.Column is >= 0 and < BoardSize;

    public Piece? this[int row, int column] => Board[row, column];
    public Piece? this[Coords coords] => Board[coords.Row, coords.Column];
}
