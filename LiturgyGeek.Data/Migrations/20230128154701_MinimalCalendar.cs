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
                name: "Definition",
                table: "Calendars",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ChurchRuleId",
                table: "CalendarItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChurchRule",
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
                    table.PrimaryKey("PK_ChurchRule", x => x.ChurchRuleId);
                    table.ForeignKey(
                        name: "FK_ChurchRule_Calendars_CalendarId",
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
                name: "IX_ChurchRule_CalendarId_RuleGroupCode_RuleCode",
                table: "ChurchRule",
                columns: new[] { "CalendarId", "RuleGroupCode", "RuleCode" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarItems_ChurchRule_ChurchRuleId",
                table: "CalendarItems",
                column: "ChurchRuleId",
                principalTable: "ChurchRule",
                principalColumn: "ChurchRuleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarItems_ChurchRule_ChurchRuleId",
                table: "CalendarItems");

            migrationBuilder.DropTable(
                name: "ChurchRule");

            migrationBuilder.DropIndex(
                name: "IX_CalendarItems_ChurchRuleId",
                table: "CalendarItems");

            migrationBuilder.DropColumn(
                name: "Definition",
                table: "Calendars");

            migrationBuilder.DropColumn(
                name: "ChurchRuleId",
                table: "CalendarItems");
        }
    }
}
