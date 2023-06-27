using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zwitscher.Migrations
{
    /// <inheritdoc />
    public partial class addedPublicFlagToPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Post",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Post");
        }
    }
}
