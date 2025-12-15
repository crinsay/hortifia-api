using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hortifia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedLikesNumberColumnFromPostsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LikesNumber",
                table: "Posts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LikesNumber",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
