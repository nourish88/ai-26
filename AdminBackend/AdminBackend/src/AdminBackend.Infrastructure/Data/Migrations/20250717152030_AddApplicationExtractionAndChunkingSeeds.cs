using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminBackend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationExtractionAndChunkingSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "ApplicationChunkingStrategies",
                columns: new[] { "Id", "ApplicationId", "ChunkSize", "ChunkingStrategyId", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "Overlap", "Seperator", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate" },
                values: new object[] { 1L, 1L, 200, 3L, null, null, null, null, 50, null, null, null, null, null });

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "ApplicationExtractorEngines",
                columns: new[] { "Id", "ApplicationId", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "ExtractorEngineTypeId", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate" },
                values: new object[] { 1L, 1L, null, null, null, null, 1L, null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "ApplicationChunkingStrategies",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "ApplicationExtractorEngines",
                keyColumn: "Id",
                keyValue: 1L);
        }
    }
}
