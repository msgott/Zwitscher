using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zwitscher.Migrations
{
    /// <inheritdoc />
    public partial class AddedBlockedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserBlockers",
                columns: table => new
                {
                    BlockedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlockingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBlockers", x => new { x.BlockedById, x.BlockingId });
                    table.ForeignKey(
                        name: "FK_UserBlockers_User_BlockedById",
                        column: x => x.BlockedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBlockers_User_BlockingId",
                        column: x => x.BlockingId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBlockers_BlockingId",
                table: "UserBlockers",
                column: "BlockingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserBlockers");
        }
    }
}
