using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessPlatform.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedTimeControl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "BlackPlayerRemainingTime",
                table: "ChessGames",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ChessGames",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TimeControl",
                table: "ChessGames",
                type: "character varying(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "WhitePlayerRemainingTime",
                table: "ChessGames",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlackPlayerRemainingTime",
                table: "ChessGames");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ChessGames");

            migrationBuilder.DropColumn(
                name: "TimeControl",
                table: "ChessGames");

            migrationBuilder.DropColumn(
                name: "WhitePlayerRemainingTime",
                table: "ChessGames");
        }
    }
}
