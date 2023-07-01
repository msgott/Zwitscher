using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zwitscher.Migrations
{
    /// <inheritdoc />
    public partial class AddedRetweetToModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "retweetsId",
                table: "Post",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_retweetsId",
                table: "Post",
                column: "retweetsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Post_retweetsId",
                table: "Post",
                column: "retweetsId",
                principalTable: "Post",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Post_retweetsId",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_retweetsId",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "retweetsId",
                table: "Post");
        }
    }
}
