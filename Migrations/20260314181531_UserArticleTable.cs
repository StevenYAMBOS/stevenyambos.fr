using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BACK_END.Migrations
{
    /// <inheritdoc />
    public partial class UserArticleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ArticleId",
                table: "user_articles",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_articles_ArticleId",
                table: "user_articles",
                column: "ArticleId");

            migrationBuilder.AddForeignKey(
                name: "FK_user_articles_articles_ArticleId",
                table: "user_articles",
                column: "ArticleId",
                principalTable: "articles",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_articles_articles_ArticleId",
                table: "user_articles");

            migrationBuilder.DropIndex(
                name: "IX_user_articles_ArticleId",
                table: "user_articles");

            migrationBuilder.DropColumn(
                name: "ArticleId",
                table: "user_articles");
        }
    }
}
