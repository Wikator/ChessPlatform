using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessPlatform.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedLastMovePlayedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PlayedAt",
                table: "LastMoves",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayedAt",
                table: "LastMoves");
        }
    }
}
