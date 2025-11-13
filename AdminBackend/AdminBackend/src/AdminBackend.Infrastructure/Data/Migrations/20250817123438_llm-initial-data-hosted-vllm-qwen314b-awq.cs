using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminBackend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class llminitialdatahostedvllmqwen314bawq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "Llms",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "LlmProviderId", "MaxInputTokenSize", "MaxOutputTokenSize", "ModelName", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate", "Url" },
                values: new object[] { 10L, null, null, null, null, 1L, 32000, 16000, "hosted_vllm/Qwen/Qwen3-14B-AWQ", null, null, null, null, "http://127.0.0.1:4000" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "Llms",
                keyColumn: "Id",
                keyValue: 10L);
        }
    }
}
