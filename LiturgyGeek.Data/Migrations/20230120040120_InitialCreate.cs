using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiturgyGeek.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calendars",
                columns: table => new
                {
                    CalendarId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendars", x => x.CalendarId);
                });

            migrationBuilder.CreateTable(
                name: "Occasions",
                columns: table => new
                {
                    OccasionId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DefaultName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Occasions", x => x.OccasionId);
                });

            migrationBuilder.CreateTable(
                name: "CalendarItems",
                columns: table => new
                {
                    CalendarId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    OccasionId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TransferredFrom = table.Column<DateTime>(type: "date", nullable: true),
                    CustomFlags = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarItems", x => new { x.CalendarId, x.Date, x.DisplayOrder });
                    table.ForeignKey(
                        name: "FK_CalendarItems_Calendars_CalendarId",
                        column: x => x.CalendarId,
                        principalTable: "Calendars",
                        principalColumn: "CalendarId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CalendarItems_Occasions_OccasionId",
                        column: x => x.OccasionId,
                        principalTable: "Occasions",
                        principalColumn: "OccasionId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarItems_OccasionId",
                table: "CalendarItems",
                column: "OccasionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalendarItems");

            migrationBuilder.DropTable(
                name: "Calendars");

            migrationBuilder.DropTable(
                name: "Occasions");
        }
    }
}
