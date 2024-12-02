using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Localizard.Migrations
{
    /// <inheritdoc />
    public partial class PluralsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "Plurals",
                table: "Languages",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Plurals",
                table: "Languages");
        }
    }
}
