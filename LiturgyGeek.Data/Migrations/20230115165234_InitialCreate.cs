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
            migrationBuilder.EnsureSchema(
                name: "calendar");

            migrationBuilder.CreateTable(
                name: "Occasions",
                schema: "calendar",
                columns: table => new
                {
                    OccasionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OccasionCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DefaultName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Occasions", x => x.OccasionId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Occasions_OccasionCode",
                schema: "calendar",
                table: "Occasions",
                column: "OccasionCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Occasions",
                schema: "calendar");
        }
    }
}
