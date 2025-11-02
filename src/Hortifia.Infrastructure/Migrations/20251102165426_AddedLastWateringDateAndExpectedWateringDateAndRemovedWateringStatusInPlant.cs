using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hortifia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedLastWateringDateAndExpectedWateringDateAndRemovedWateringStatusInPlant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WateringStatus",
                table: "Plants");

            migrationBuilder.RenameColumn(
                name: "WateringDate",
                table: "Plants",
                newName: "LastWateringDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedWateringDate",
                table: "Plants",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpectedWateringDate",
                table: "Plants");

            migrationBuilder.RenameColumn(
                name: "LastWateringDate",
                table: "Plants",
                newName: "WateringDate");

            migrationBuilder.AddColumn<byte>(
                name: "WateringStatus",
                table: "Plants",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
