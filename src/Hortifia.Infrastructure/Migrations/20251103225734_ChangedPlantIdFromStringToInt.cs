using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hortifia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedPlantIdFromStringToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                           name: "Plants");

            migrationBuilder.CreateTable(
                name: "Plants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    ImgBlobName = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    IsNearHeater = table.Column<bool>(type: "bit", nullable: false),
                    LightCondition = table.Column<int>(type: "int", nullable: false),
                    LastWateringDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsFavourite = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    CommonName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ExpectedWateringDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plants_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Plants_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Plants_RoomId",
                table: "Plants",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Plants_UserId",
                table: "Plants",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                 name: "Plants");

            migrationBuilder.CreateTable(
                name: "Plants",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    ImgBlobName = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    IsNearHeater = table.Column<bool>(type: "bit", nullable: false),
                    LightCondition = table.Column<int>(type: "int", nullable: false),
                    LastWateringDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsFavourite = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    CommonName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ExpectedWateringDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },

                constraints: table =>
                {
                    table.PrimaryKey("PK_Plants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plants_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Plants_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Plants_RoomId",
                table: "Plants",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Plants_UserId",
                table: "Plants",
                column: "UserId");
        }
    }
}
