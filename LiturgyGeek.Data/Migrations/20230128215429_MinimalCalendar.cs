using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiturgyGeek.Data.Migrations
{
    /// <inheritdoc />
    public partial class MinimalCalendar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TraditionCode",
                table: "Calendars",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ChurchRuleId",
                table: "CalendarItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CalendarDefinitions",
                columns: table => new
                {
                    CalendarId = table.Column<int>(type: "int", nullable: false),
                    Definition = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarDefinitions", x => x.CalendarId);
                    table.ForeignKey(
                        name: "FK_CalendarDefinitions_Calendars_CalendarId",
                        column: x => x.CalendarId,
                        principalTable: "Calendars",
                        principalColumn: "CalendarId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChurchRules",
                columns: table => new
                {
                    ChurchRuleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalendarId = table.Column<int>(type: "int", nullable: false),
                    RuleGroupCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RuleCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Elaboration = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChurchRules", x => x.ChurchRuleId);
                    table.ForeignKey(
                        name: "FK_ChurchRules_Calendars_CalendarId",
                        column: x => x.CalendarId,
                        principalTable: "Calendars",
                        principalColumn: "CalendarId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarItems_ChurchRuleId",
                table: "CalendarItems",
                column: "ChurchRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ChurchRules_CalendarId_RuleGroupCode_RuleCode",
                table: "ChurchRules",
                columns: new[] { "CalendarId", "RuleGroupCode", "RuleCode" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarItems_ChurchRules_ChurchRuleId",
                table: "CalendarItems",
                column: "ChurchRuleId",
                principalTable: "ChurchRules",
                principalColumn: "ChurchRuleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarItems_ChurchRules_ChurchRuleId",
                table: "CalendarItems");

            migrationBuilder.DropTable(
                name: "CalendarDefinitions");

            migrationBuilder.DropTable(
                name: "ChurchRules");

            migrationBuilder.DropIndex(
                name: "IX_CalendarItems_ChurchRuleId",
                table: "CalendarItems");

            migrationBuilder.DropColumn(
                name: "TraditionCode",
                table: "Calendars");

            migrationBuilder.DropColumn(
                name: "ChurchRuleId",
                table: "CalendarItems");
        }
    }
}
