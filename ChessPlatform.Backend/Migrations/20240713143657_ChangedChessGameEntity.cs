using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessPlatform.Backend.Migrations
{
    /// <inheritdoc />
    public partial class ChangedChessGameEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChessGames_ChessBoardEntity_ChessBoardId",
                table: "ChessGames");

            migrationBuilder.DropTable(
                name: "PieceEntity");

            migrationBuilder.DropTable(
                name: "ChessBoardEntity");

            migrationBuilder.DropIndex(
                name: "IX_ChessGames_ChessBoardId",
                table: "ChessGames");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LastMoveEntity",
                table: "LastMoveEntity");

            migrationBuilder.DropColumn(
                name: "ChessBoardId",
                table: "ChessGames");

            migrationBuilder.RenameTable(
                name: "LastMoveEntity",
                newName: "LastMoves");

            migrationBuilder.AddColumn<string>(
                name: "Fen",
                table: "ChessGames",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GameOverStatus",
                table: "ChessGames",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOver",
                table: "ChessGames",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "LastMoveId",
                table: "ChessGames",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlayerTurn",
                table: "ChessGames",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Winner",
                table: "ChessGames",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LastMoves",
                table: "LastMoves",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ChessGames_LastMoveId",
                table: "ChessGames",
                column: "LastMoveId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChessGames_LastMoves_LastMoveId",
                table: "ChessGames",
                column: "LastMoveId",
                principalTable: "LastMoves",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChessGames_LastMoves_LastMoveId",
                table: "ChessGames");

            migrationBuilder.DropIndex(
                name: "IX_ChessGames_LastMoveId",
                table: "ChessGames");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LastMoves",
                table: "LastMoves");

            migrationBuilder.DropColumn(
                name: "Fen",
                table: "ChessGames");

            migrationBuilder.DropColumn(
                name: "GameOverStatus",
                table: "ChessGames");

            migrationBuilder.DropColumn(
                name: "IsOver",
                table: "ChessGames");

            migrationBuilder.DropColumn(
                name: "LastMoveId",
                table: "ChessGames");

            migrationBuilder.DropColumn(
                name: "PlayerTurn",
                table: "ChessGames");

            migrationBuilder.DropColumn(
                name: "Winner",
                table: "ChessGames");

            migrationBuilder.RenameTable(
                name: "LastMoves",
                newName: "LastMoveEntity");

            migrationBuilder.AddColumn<Guid>(
                name: "ChessBoardId",
                table: "ChessGames",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_LastMoveEntity",
                table: "LastMoveEntity",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ChessBoardEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LastMoveId = table.Column<Guid>(type: "uuid", nullable: true),
                    PlayerTurn = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChessBoardEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChessBoardEntity_LastMoveEntity_LastMoveId",
                        column: x => x.LastMoveId,
                        principalTable: "LastMoveEntity",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PieceEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChessBoardId = table.Column<Guid>(type: "uuid", nullable: false),
                    Color = table.Column<int>(type: "integer", nullable: false),
                    Column = table.Column<int>(type: "integer", nullable: false),
                    FENChar = table.Column<int>(type: "integer", nullable: false),
                    HasMoved = table.Column<bool>(type: "boolean", nullable: true),
                    Row = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PieceEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PieceEntity_ChessBoardEntity_ChessBoardId",
                        column: x => x.ChessBoardId,
                        principalTable: "ChessBoardEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChessGames_ChessBoardId",
                table: "ChessGames",
                column: "ChessBoardId");

            migrationBuilder.CreateIndex(
                name: "IX_ChessBoardEntity_LastMoveId",
                table: "ChessBoardEntity",
                column: "LastMoveId");

            migrationBuilder.CreateIndex(
                name: "IX_PieceEntity_ChessBoardId",
                table: "PieceEntity",
                column: "ChessBoardId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChessGames_ChessBoardEntity_ChessBoardId",
                table: "ChessGames",
                column: "ChessBoardId",
                principalTable: "ChessBoardEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
