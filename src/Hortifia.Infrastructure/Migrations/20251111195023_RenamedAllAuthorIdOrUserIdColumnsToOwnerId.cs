using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hortifia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenamedAllAuthorIdOrUserIdColumnsToOwnerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plants_AspNetUsers_UserId",
                table: "Plants");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_AuthorId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_AspNetUsers_UserId",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Rooms",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_UserId",
                table: "Rooms",
                newName: "IX_Rooms_OwnerId");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Posts",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_AuthorId",
                table: "Posts",
                newName: "IX_Posts_OwnerId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Plants",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Plants_UserId",
                table: "Plants",
                newName: "IX_Plants_OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Plants_AspNetUsers_OwnerId",
                table: "Plants",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_OwnerId",
                table: "Posts",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_AspNetUsers_OwnerId",
                table: "Rooms",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plants_AspNetUsers_OwnerId",
                table: "Plants");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_OwnerId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_AspNetUsers_OwnerId",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Rooms",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_OwnerId",
                table: "Rooms",
                newName: "IX_Rooms_UserId");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Posts",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_OwnerId",
                table: "Posts",
                newName: "IX_Posts_AuthorId");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Plants",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Plants_OwnerId",
                table: "Plants",
                newName: "IX_Plants_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Plants_AspNetUsers_UserId",
                table: "Plants",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_AuthorId",
                table: "Posts",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_AspNetUsers_UserId",
                table: "Rooms",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
