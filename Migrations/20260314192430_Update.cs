using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BACK_END.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_articles_users_users",
                table: "articles");

            migrationBuilder.RenameColumn(
                name: "users",
                table: "articles",
                newName: "articles");

            migrationBuilder.RenameIndex(
                name: "IX_articles_users",
                table: "articles",
                newName: "IX_articles_articles");

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
    }
}
