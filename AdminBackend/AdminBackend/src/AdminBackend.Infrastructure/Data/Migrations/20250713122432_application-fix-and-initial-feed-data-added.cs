using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminBackend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class applicationfixandinitialfeeddataadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_ChunkingStrategies_ChunkingStrategyId",
                schema: "jugaai",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_ChunkingStrategyId",
                schema: "jugaai",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationChunkingStrategies_ApplicationId",
                schema: "jugaai",
                table: "ApplicationChunkingStrategies");

            migrationBuilder.DropColumn(
                name: "ChunkingStrategyId",
                schema: "jugaai",
                table: "Applications");

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "Applications",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "HasApplicationFile", "HasUserFile", "Identifier", "Name", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate" },
                values: new object[] { 1L, null, null, null, null, true, false, "test-app", "Test App", null, null, null, null });

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "Embeddings",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "LlmProviderId", "MaxInputTokenSize", "ModelName", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate", "Url", "VectorSize" },
                values: new object[] { 1L, null, null, null, null, 1L, 32000, "Qwen3-Embedding-4B", null, null, null, null, "http://127.0.0.1:4000", 2560 });

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "SearchEngines",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "Identifier", "SearchEngineTypeId", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate", "Url" },
                values: new object[] { 1L, null, null, null, null, "elastic-local", 1L, null, null, null, null, "http://127.0.0.1:9200" });

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "ApplicationEmbeddings",
                columns: new[] { "Id", "ApplicationId", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "EmbeddingId", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate" },
                values: new object[] { 1L, 1L, null, null, null, null, 1L, null, null, null, null });

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "ApplicationSearchEngines",
                columns: new[] { "Id", "ApplicationId", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "EmbeddingId", "Identifier", "IndexName", "SearchEngineId", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate", "VectorSize" },
                values: new object[] { 1L, 1L, null, null, null, null, 1L, "test-app-local-index", "test-app-index", 1L, null, null, null, null, 2560 });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationChunkingStrategies_ApplicationId",
                schema: "jugaai",
                table: "ApplicationChunkingStrategies",
                column: "ApplicationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationChunkingStrategies_ApplicationId",
                schema: "jugaai",
                table: "ApplicationChunkingStrategies");

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "ApplicationEmbeddings",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "ApplicationSearchEngines",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "Applications",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "Embeddings",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "SearchEngines",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.AddColumn<long>(
                name: "ChunkingStrategyId",
                schema: "jugaai",
                table: "Applications",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ChunkingStrategyId",
                schema: "jugaai",
                table: "Applications",
                column: "ChunkingStrategyId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationChunkingStrategies_ApplicationId",
                schema: "jugaai",
                table: "ApplicationChunkingStrategies",
                column: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_ChunkingStrategies_ChunkingStrategyId",
                schema: "jugaai",
                table: "Applications",
                column: "ChunkingStrategyId",
                principalSchema: "jugaai",
                principalTable: "ChunkingStrategies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
