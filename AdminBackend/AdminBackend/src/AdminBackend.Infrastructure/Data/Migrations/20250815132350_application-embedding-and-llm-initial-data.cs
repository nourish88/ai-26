using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminBackend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class applicationembeddingandllminitialdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "ApplicationEmbeddings",
                keyColumn: "Id",
                keyValue: 1L,
                column: "EmbeddingId",
                value: 3L);

            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "ApplicationLlms",
                keyColumn: "Id",
                keyValue: 1L,
                column: "LlmId",
                value: 6L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "ApplicationEmbeddings",
                keyColumn: "Id",
                keyValue: 1L,
                column: "EmbeddingId",
                value: 1L);

            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "ApplicationLlms",
                keyColumn: "Id",
                keyValue: 1L,
                column: "LlmId",
                value: 1L);
        }
    }
}
