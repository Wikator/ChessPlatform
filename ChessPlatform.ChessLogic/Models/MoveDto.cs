namespace ChessPlatform.ChessLogic.Models;

public class MoveDto(int fromRow, int fromColumn, int toRow, int toColumn)
{
    private int FromRow { get; } = fromRow;
    private int FromColumn { get; } = fromColumn;
    private int ToRow { get; } = toRow;
    private int ToColumn { get; } = toColumn;
    
    public Coords From => new Coords(FromRow, FromColumn);
    public Coords To => new Coords(ToRow, ToColumn);

    public MoveDto(Coords from, Coords to) : this(from.Row,
        from.Column, to.Row, to.Column)
    {
    }
}