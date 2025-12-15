using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hortifia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixedLongtitudeTypoInColumnNameToCorrectLongitudeInAspNetUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Coordinates_Longtitude",
                table: "AspNetUsers",
                newName: "Coordinates_Longitude");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Coordinates_Longitude",
                table: "AspNetUsers",
                newName: "Coordinates_Longtitude");
        }
    }
}
