using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AdminBackend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class llmsinitialdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "Llms",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "MaxInputTokenSize", "ModelName" },
                values: new object[] { 32768, "ollama/qwen3:14b" });

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "Llms",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "LlmProviderId", "MaxInputTokenSize", "MaxOutputTokenSize", "ModelName", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate", "Url" },
                values: new object[,]
                {
                    { 2L, null, null, null, null, 1L, 32000, 16000, "ollama/qwen3:30b", null, null, null, null, "http://127.0.0.1:4000" },
                    { 3L, null, null, null, null, 1L, 32000, 16000, "ollama/qwen3:32b", null, null, null, null, "http://127.0.0.1:4000" },
                    { 4L, null, null, null, null, 1L, 32000, 16000, "ollama/deepseek-r1:32b", null, null, null, null, "http://127.0.0.1:4000" },
                    { 5L, null, null, null, null, 1L, 32000, 16000, "remote-ollama/qwen3:14b", null, null, null, null, "http://127.0.0.1:4000" },
                    { 6L, null, null, null, null, 1L, 32000, 16000, "remote-ollama/qwen3:32b", null, null, null, null, "http://127.0.0.1:4000" },
                    { 7L, null, null, null, null, 1L, 32000, 16000, "remote-ollama/gpt-oss:20b", null, null, null, null, "http://127.0.0.1:4000" },
                    { 8L, null, null, null, null, 1L, 32000, 16000, "ollama/gpt-oss:20b", null, null, null, null, "http://127.0.0.1:4000" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "Llms",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "Llms",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "Llms",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "Llms",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "Llms",
                keyColumn: "Id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "Llms",
                keyColumn: "Id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "Llms",
                keyColumn: "Id",
                keyValue: 8L);

            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "Llms",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "MaxInputTokenSize", "ModelName" },
                values: new object[] { 32000, "Qwen/Qwen3-32B-AWQ" });
        }
    }
}
