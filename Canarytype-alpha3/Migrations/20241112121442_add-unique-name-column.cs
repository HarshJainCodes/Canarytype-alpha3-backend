using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Canarytype_alpha3.Migrations
{
    /// <inheritdoc />
    public partial class adduniquenamecolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UniqueName",
                table: "UsersTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UniqueName",
                table: "UsersTable");
        }
    }
}
