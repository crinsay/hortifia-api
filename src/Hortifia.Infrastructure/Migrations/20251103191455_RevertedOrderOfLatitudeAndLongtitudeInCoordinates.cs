using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hortifia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RevertedOrderOfLatitudeAndLongtitudeInCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Coordinates_Longtitude",
                table: "AspNetUsers",
                newName: "Coordinates_Temp");

            migrationBuilder.RenameColumn(
                name: "Coordinates_Latitude",
                table: "AspNetUsers",
                newName: "Coordinates_Longtitude");

            migrationBuilder.RenameColumn(
                name: "Coordinates_Temp",
                table: "AspNetUsers",
                newName: "Coordinates_Latitude");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Coordinates_Latitude",
                table: "AspNetUsers",
                newName: "Coordinates_Temp");

            migrationBuilder.RenameColumn(
                name: "Coordinates_Longtitude",
                table: "AspNetUsers",
                newName: "Coordinates_Latitude");

            migrationBuilder.RenameColumn(
                name: "Coordinates_Temp",
                table: "AspNetUsers",
                newName: "Coordinates_Longtitude");
        }
    }
}
