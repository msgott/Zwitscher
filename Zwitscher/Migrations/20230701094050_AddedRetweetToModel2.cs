using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zwitscher.Migrations
{
    /// <inheritdoc />
    public partial class AddedRetweetToModel2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Post_retweetsId",
                table: "Post");

            migrationBuilder.RenameColumn(
                name: "retweetsId",
                table: "Post",
                newName: "retweetsID");

            migrationBuilder.RenameIndex(
                name: "IX_Post_retweetsId",
                table: "Post",
                newName: "IX_Post_retweetsID");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Post_retweetsID",
                table: "Post",
                column: "retweetsID",
                principalTable: "Post",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Post_retweetsID",
                table: "Post");

            migrationBuilder.RenameColumn(
                name: "retweetsID",
                table: "Post",
                newName: "retweetsId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_retweetsID",
                table: "Post",
                newName: "IX_Post_retweetsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Post_retweetsId",
                table: "Post",
                column: "retweetsId",
                principalTable: "Post",
                principalColumn: "Id");
        }
    }
}
