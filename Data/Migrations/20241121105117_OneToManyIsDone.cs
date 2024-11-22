using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Localizard.Migrations
{
    /// <inheritdoc />
    public partial class OneToManyIsDone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Translations_LanguageId",
                table: "Translations");

            migrationBuilder.CreateIndex(
                name: "IX_Translations_LanguageId",
                table: "Translations",
                column: "LanguageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Translations_LanguageId",
                table: "Translations");

            migrationBuilder.CreateIndex(
                name: "IX_Translations_LanguageId",
                table: "Translations",
                column: "LanguageId",
                unique: true);
        }
    }
}
