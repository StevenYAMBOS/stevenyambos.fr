using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BACK_END.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_articles_users_author",
                table: "articles");

            migrationBuilder.DropIndex(
                name: "IX_articles_author",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
