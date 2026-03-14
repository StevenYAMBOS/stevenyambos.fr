using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BACK_END.Migrations
{
    /// <inheritdoc />
    public partial class Update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_articles_users_articles",
                table: "articles");

            migrationBuilder.DropIndex(
                name: "IX_articles_articles",
                table: "articles");

            migrationBuilder.DropColumn(
                name: "articles",
                table: "articles");

            migrationBuilder.CreateIndex(
                name: "IX_articles_author_id",
                table: "articles",
                column: "author_id");

            migrationBuilder.AddForeignKey(
                name: "FK_articles_users_author_id",
                table: "articles",
                column: "author_id",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_articles_users_author_id",
                table: "articles");

            migrationBuilder.DropIndex(
                name: "IX_articles_author_id",
                table: "articles");

            migrationBuilder.AddColumn<string>(
                name: "articles",
                table: "articles",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_articles_articles",
                table: "articles",
                column: "articles");

            migrationBuilder.AddForeignKey(
                name: "FK_articles_users_articles",
                table: "articles",
                column: "articles",
                principalTable: "users",
                principalColumn: "Id");
        }
    }
}
