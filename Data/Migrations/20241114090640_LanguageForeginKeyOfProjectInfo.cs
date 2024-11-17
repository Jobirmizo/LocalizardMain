using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Localizard.Migrations
{
    /// <inheritdoc />
    public partial class LanguageForeginKeyOfProjectInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Languages_Projects_ProjectInfoId",
                table: "Languages");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectDetails_Translations_TranslationId",
                table: "ProjectDetails");

            migrationBuilder.DropIndex(
                name: "IX_Languages_ProjectInfoId",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "ProjectInfoId",
                table: "Languages");

            migrationBuilder.AlterColumn<int>(
                name: "TranslationId",
                table: "ProjectDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "LanguageProjectInfo",
                columns: table => new
                {
                    LanguagesId = table.Column<int>(type: "integer", nullable: false),
                    ProjectInfosId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageProjectInfo", x => new { x.LanguagesId, x.ProjectInfosId });
                    table.ForeignKey(
                        name: "FK_LanguageProjectInfo_Languages_LanguagesId",
                        column: x => x.LanguagesId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LanguageProjectInfo_Projects_ProjectInfosId",
                        column: x => x.ProjectInfosId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LanguageProjectInfo_ProjectInfosId",
                table: "LanguageProjectInfo",
                column: "ProjectInfosId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectDetails_Translations_TranslationId",
                table: "ProjectDetails",
                column: "TranslationId",
                principalTable: "Translations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectDetails_Translations_TranslationId",
                table: "ProjectDetails");

            migrationBuilder.DropTable(
                name: "LanguageProjectInfo");

            migrationBuilder.AlterColumn<int>(
                name: "TranslationId",
                table: "ProjectDetails",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "ProjectInfoId",
                table: "Languages",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_ProjectInfoId",
                table: "Languages",
                column: "ProjectInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Languages_Projects_ProjectInfoId",
                table: "Languages",
                column: "ProjectInfoId",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectDetails_Translations_TranslationId",
                table: "ProjectDetails",
                column: "TranslationId",
                principalTable: "Translations",
                principalColumn: "Id");
        }
    }
}
