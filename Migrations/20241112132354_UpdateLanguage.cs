using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Localizard.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Languages_Projects_ProjectInfoId1",
                table: "Languages");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Languages_LanguageId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_LanguageId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectDetailId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Languages_ProjectInfoId1",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "ProjectInfoId1",
                table: "Languages");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectDetailId",
                table: "Projects",
                column: "ProjectDetailId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectDetailId",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "ProjectInfoId1",
                table: "Languages",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_LanguageId",
                table: "Projects",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectDetailId",
                table: "Projects",
                column: "ProjectDetailId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_ProjectInfoId1",
                table: "Languages",
                column: "ProjectInfoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Languages_Projects_ProjectInfoId1",
                table: "Languages",
                column: "ProjectInfoId1",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Languages_LanguageId",
                table: "Projects",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
