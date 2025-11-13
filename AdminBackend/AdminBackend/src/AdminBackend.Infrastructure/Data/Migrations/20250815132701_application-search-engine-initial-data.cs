using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminBackend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class applicationsearchengineinitialdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "ApplicationSearchEngines",
                keyColumn: "Id",
                keyValue: 1L,
                column: "EmbeddingId",
                value: 3L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "ApplicationSearchEngines",
                keyColumn: "Id",
                keyValue: 1L,
                column: "EmbeddingId",
                value: 1L);
        }
    }
}
