using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AdminBackend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class embeddingsinitialdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "Embeddings",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "MaxInputTokenSize", "ModelName" },
                values: new object[] { 32768, "lm_studio/text-embedding-qwen3-embedding-4b" });

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "Embeddings",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "LlmProviderId", "MaxInputTokenSize", "ModelName", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate", "Url", "VectorSize" },
                values: new object[,]
                {
                    { 2L, null, null, null, null, 1L, 32768, "ollama/dengcao/Qwen3-Embedding-8B:Q8_0", null, null, null, null, "http://127.0.0.1:4000", 2560 },
                    { 3L, null, null, null, null, 1L, 32768, "remote-ollama/dengcao/Qwen3-Embedding-4B:Q8_0", null, null, null, null, "http://127.0.0.1:4000", 2560 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "Embeddings",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "Embeddings",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "Embeddings",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "MaxInputTokenSize", "ModelName" },
                values: new object[] { 32000, "Qwen3-Embedding-4B" });
        }
    }
}
