using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BACK_END.Migrations
{
    /// <inheritdoc />
    public partial class FixArticleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "author",
                table: "articles",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_articles_author",
                table: "articles",
                column: "author");

            migrationBuilder.AddForeignKey(
                name: "FK_articles_users_author",
                table: "articles",
                column: "author",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_articles_users_author",
                table: "articles");

            migrationBuilder.DropIndex(
                name: "IX_articles_author",
                table: "articles");

            migrationBuilder.DropColumn(
                name: "author",
                table: "articles");
        }
    }
}
