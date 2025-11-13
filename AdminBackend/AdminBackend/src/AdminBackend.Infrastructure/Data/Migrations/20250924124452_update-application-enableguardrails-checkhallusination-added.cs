using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminBackend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateapplicationenableguardrailscheckhallusinationadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CheckHallucination",
                schema: "jugaai",
                table: "Applications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnableGuardRails",
                schema: "jugaai",
                table: "Applications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "Applications",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CheckHallucination", "EnableGuardRails" },
                values: new object[] { true, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckHallucination",
                schema: "jugaai",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "EnableGuardRails",
                schema: "jugaai",
                table: "Applications");
        }
    }
}
