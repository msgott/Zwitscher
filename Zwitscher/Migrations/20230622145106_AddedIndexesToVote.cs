using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zwitscher.Migrations
{
    /// <inheritdoc />
    public partial class AddedIndexesToVote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vote_UserId",
                table: "Vote");

            migrationBuilder.CreateIndex(
                name: "IX_Vote_UserId_PostId",
                table: "Vote",
                columns: new[] { "UserId", "PostId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vote_UserId_PostId",
                table: "Vote");

            migrationBuilder.CreateIndex(
                name: "IX_Vote_UserId",
                table: "Vote",
                column: "UserId");
        }
    }
}
