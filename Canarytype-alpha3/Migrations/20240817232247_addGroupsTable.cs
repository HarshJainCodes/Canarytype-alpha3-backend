using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Canarytype_alpha3.Migrations
{
    /// <inheritdoc />
    public partial class addGroupsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MatchInfoTable",
                columns: table => new
                {
                    RoomId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Player1Id = table.Column<int>(type: "int", nullable: false),
                    Player2Id = table.Column<int>(type: "int", nullable: false),
                    matchDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Player1Submissions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Player2Submissions = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchInfoTable", x => x.RoomId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchInfoTable");
        }
    }
}
