using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Canarytype_alpha3.Migrations
{
    /// <inheritdoc />
    public partial class AddTypingSpeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "rawTypingSpeedPerSecond",
                table: "UsersSubmissionsTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "typingSpeedPerSecond",
                table: "UsersSubmissionsTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "rawTypingSpeedPerSecond",
                table: "UsersSubmissionsTable");

            migrationBuilder.DropColumn(
                name: "typingSpeedPerSecond",
                table: "UsersSubmissionsTable");
        }
    }
}
