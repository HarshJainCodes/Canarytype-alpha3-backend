using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Canarytype_alpha3.Migrations
{
    /// <inheritdoc />
    public partial class add_fk_matchinfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MatchInfoTable_Player1Id",
                table: "MatchInfoTable",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_MatchInfoTable_Player2Id",
                table: "MatchInfoTable",
                column: "Player2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchInfoTable_UsersTable_Player1Id",
                table: "MatchInfoTable",
                column: "Player1Id",
                principalTable: "UsersTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchInfoTable_UsersTable_Player2Id",
                table: "MatchInfoTable",
                column: "Player2Id",
                principalTable: "UsersTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchInfoTable_UsersTable_Player1Id",
                table: "MatchInfoTable");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchInfoTable_UsersTable_Player2Id",
                table: "MatchInfoTable");

            migrationBuilder.DropIndex(
                name: "IX_MatchInfoTable_Player1Id",
                table: "MatchInfoTable");

            migrationBuilder.DropIndex(
                name: "IX_MatchInfoTable_Player2Id",
                table: "MatchInfoTable");
        }
    }
}
