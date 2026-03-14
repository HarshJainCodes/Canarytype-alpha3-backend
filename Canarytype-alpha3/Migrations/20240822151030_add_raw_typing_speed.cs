using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Canarytype_alpha3.Migrations
{
    /// <inheritdoc />
    public partial class add_raw_typing_speed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Player2Submissions",
                table: "MatchInfoTable",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Player1Submissions",
                table: "MatchInfoTable",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Player1SubmissionsRaw",
                table: "MatchInfoTable",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Player2SubmissionsRaw",
                table: "MatchInfoTable",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Player1SubmissionsRaw",
                table: "MatchInfoTable");

            migrationBuilder.DropColumn(
                name: "Player2SubmissionsRaw",
                table: "MatchInfoTable");

            migrationBuilder.AlterColumn<string>(
                name: "Player2Submissions",
                table: "MatchInfoTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Player1Submissions",
                table: "MatchInfoTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
