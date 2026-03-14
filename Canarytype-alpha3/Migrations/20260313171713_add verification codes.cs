using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Canarytype_alpha3.Migrations
{
    /// <inheritdoc />
    public partial class addverificationcodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserEmail",
                table: "UsersTable",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "VerificationCodes",
                columns: table => new
                {
                    UserEmail = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OTP = table.Column<int>(type: "int", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationCodes", x => x.UserEmail);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersTable_UserEmail",
                table: "UsersTable",
                column: "UserEmail",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VerificationCodes");

            migrationBuilder.DropIndex(
                name: "IX_UsersTable_UserEmail",
                table: "UsersTable");

            migrationBuilder.AlterColumn<string>(
                name: "UserEmail",
                table: "UsersTable",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
