namespace ChessPlatform.Models.Chess.Pieces;

public enum GameOverStatus
{
    Checkmate,
    TimeOut,
    Resignation,
    Stalemate,
    DrawAgreement,
    InsufficientMaterial,
    ThreefoldRepetition,
    FiftyMoveRule,
    Abandoned
}