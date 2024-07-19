using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessPlatform.Backend.Migrations
{
    /// <inheritdoc />
    public partial class ChangedFenMaxLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Fen",
                table: "ChessGames",
                type: "character varying(87)",
                maxLength: 87,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Fen",
                table: "ChessGames",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(87)",
                oldMaxLength: 87);
        }
    }
}
