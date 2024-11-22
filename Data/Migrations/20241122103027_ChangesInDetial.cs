using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Localizard.Migrations
{
    /// <inheritdoc />
    public partial class ChangesInDetial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "ProjectDetailTranslation",
                columns: table => new
                {
                    ProjectDetailsId = table.Column<int>(type: "integer", nullable: false),
                    TranslationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectDetailTranslation", x => new { x.ProjectDetailsId, x.TranslationId });
                    table.ForeignKey(
                        name: "FK_ProjectDetailTranslation_ProjectDetails_ProjectDetailsId",
                        column: x => x.ProjectDetailsId,
                        principalTable: "ProjectDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectDetailTranslation_Translations_TranslationId",
                        column: x => x.TranslationId,
                        principalTable: "Translations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDetailTranslation_TranslationId",
                table: "ProjectDetailTranslation",
                column: "TranslationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectDetailTranslation");

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
    }
}
