using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Localizard.Migrations
{
    /// <inheritdoc />
    public partial class TranslationIsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectInfoId",
                table: "Translations",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Translations_ProjectInfoId",
                table: "Translations",
                column: "ProjectInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Translations_Projects_ProjectInfoId",
                table: "Translations",
                column: "ProjectInfoId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Translations_Projects_ProjectInfoId",
                table: "Translations");

            migrationBuilder.DropIndex(
                name: "IX_Translations_ProjectInfoId",
                table: "Translations");

            migrationBuilder.DropColumn(
                name: "ProjectInfoId",
                table: "Translations");
        }
    }
}
