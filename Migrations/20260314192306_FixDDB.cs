using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BACK_END.Migrations
{
    /// <inheritdoc />
    public partial class FixDDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_articles_users_articles",
                table: "articles");

            migrationBuilder.DropTable(
                name: "user_articles");

            migrationBuilder.RenameColumn(
                name: "author",
                table: "articles",
                newName: "author_id");

            migrationBuilder.RenameColumn(
                name: "articles",
                table: "articles",
                newName: "users");

            migrationBuilder.RenameIndex(
                name: "IX_articles_articles",
                table: "articles",
                newName: "IX_articles_users");

            migrationBuilder.AddForeignKey(
                name: "FK_articles_users_users",
                table: "articles",
                column: "users",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_articles_users_users",
                table: "articles");

            migrationBuilder.RenameColumn(
                name: "author_id",
                table: "articles",
                newName: "author");

            migrationBuilder.RenameColumn(
                name: "users",
                table: "articles",
                newName: "articles");

            migrationBuilder.RenameIndex(
                name: "IX_articles_users",
                table: "articles",
                newName: "IX_articles_articles");

            migrationBuilder.CreateTable(
                name: "user_articles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uuid", nullable: true),
                    user_id = table.Column<string>(type: "text", nullable: true),
                    article_id = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_articles", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_articles_articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "articles",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_articles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_articles_ArticleId",
                table: "user_articles",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_user_articles_user_id",
                table: "user_articles",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_articles_users_articles",
                table: "articles",
                column: "articles",
                principalTable: "users",
                principalColumn: "Id");
        }
    }
}
