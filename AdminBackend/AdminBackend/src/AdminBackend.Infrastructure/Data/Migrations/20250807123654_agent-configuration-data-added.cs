using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminBackend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class agentconfigurationdataadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "Llms",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "LlmProviderId", "MaxInputTokenSize", "MaxOutputTokenSize", "ModelName", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate", "Url" },
                values: new object[] { 1L, null, null, null, null, 1L, 32000, 16000, "Qwen/Qwen3-32B-AWQ", null, null, null, null, "http://127.0.0.1:4000" });

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "ApplicationLlms",
                columns: new[] { "Id", "ApplicationId", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "EnableThinking", "LlmId", "Temperature", "TopP", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate" },
                values: new object[] { 1L, 1L, null, null, null, null, false, 1L, 0f, 0.7f, null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "ApplicationLlms",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "Llms",
                keyColumn: "Id",
                keyValue: 1L);
        }
    }
}
