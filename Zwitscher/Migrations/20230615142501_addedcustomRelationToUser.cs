using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zwitscher.Migrations
{
    /// <inheritdoc />
    public partial class addedcustomRelationToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Role_RoleID",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_UserUser_User_FollowedById",
                table: "UserUser");

            migrationBuilder.DropForeignKey(
                name: "FK_UserUser_User_FollowingId",
                table: "UserUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserUser",
                table: "UserUser");

            migrationBuilder.RenameTable(
                name: "UserUser",
                newName: "UserFollowers");

            migrationBuilder.RenameIndex(
                name: "IX_UserUser_FollowingId",
                table: "UserFollowers",
                newName: "IX_UserFollowers_FollowingId");

            migrationBuilder.AlterColumn<Guid>(
                name: "RoleID",
                table: "User",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFollowers",
                table: "UserFollowers",
                columns: new[] { "FollowedById", "FollowingId" });

            migrationBuilder.AddForeignKey(
                name: "FK_User_Role_RoleID",
                table: "User",
                column: "RoleID",
                principalTable: "Role",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowers_User_FollowedById",
                table: "UserFollowers",
                column: "FollowedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowers_User_FollowingId",
                table: "UserFollowers",
                column: "FollowingId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Role_RoleID",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowers_User_FollowedById",
                table: "UserFollowers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowers_User_FollowingId",
                table: "UserFollowers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFollowers",
                table: "UserFollowers");

            migrationBuilder.RenameTable(
                name: "UserFollowers",
                newName: "UserUser");

            migrationBuilder.RenameIndex(
                name: "IX_UserFollowers_FollowingId",
                table: "UserUser",
                newName: "IX_UserUser_FollowingId");

            migrationBuilder.AlterColumn<Guid>(
                name: "RoleID",
                table: "User",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserUser",
                table: "UserUser",
                columns: new[] { "FollowedById", "FollowingId" });

            migrationBuilder.AddForeignKey(
                name: "FK_User_Role_RoleID",
                table: "User",
                column: "RoleID",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserUser_User_FollowedById",
                table: "UserUser",
                column: "FollowedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserUser_User_FollowingId",
                table: "UserUser",
                column: "FollowingId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
