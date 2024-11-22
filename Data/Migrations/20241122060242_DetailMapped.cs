using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Localizard.Migrations
{
    /// <inheritdoc />
    public partial class DetailMapped : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectDetails_Translations_TranslationId",
                table: "ProjectDetails");

            migrationBuilder.DropIndex(
                name: "IX_ProjectDetails_TranslationId",
                table: "ProjectDetails");

            migrationBuilder.AddColumn<int>(
                name: "ProjectDetailId",
                table: "Translations",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Translations_ProjectDetailId",
                table: "Translations",
                column: "ProjectDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Translations_ProjectDetails_ProjectDetailId",
                table: "Translations",
                column: "ProjectDetailId",
                principalTable: "ProjectDetails",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Translations_ProjectDetails_ProjectDetailId",
                table: "Translations");

            migrationBuilder.DropIndex(
                name: "IX_Translations_ProjectDetailId",
                table: "Translations");

            migrationBuilder.DropColumn(
                name: "ProjectDetailId",
                table: "Translations");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDetails_TranslationId",
                table: "ProjectDetails",
                column: "TranslationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectDetails_Translations_TranslationId",
                table: "ProjectDetails",
                column: "TranslationId",
                principalTable: "Translations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
