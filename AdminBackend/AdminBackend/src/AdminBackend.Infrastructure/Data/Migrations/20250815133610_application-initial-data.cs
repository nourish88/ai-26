using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminBackend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class applicationinitialdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "ApplicationChunkingStrategies",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ChunkSize", "Overlap" },
                values: new object[] { 2000, 200 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "ApplicationChunkingStrategies",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ChunkSize", "Overlap" },
                values: new object[] { 200, 50 });
        }
    }
}
