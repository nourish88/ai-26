using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminBackend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateapplicationtypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "ApplicationTypes",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Identifier",
                value: "CHATBOT");

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "ApplicationTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "Identifier", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate" },
                values: new object[] { 5L, null, null, null, null, "MCP_POWERED_AGENTIC_RAG", null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "ApplicationTypes",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "ApplicationTypes",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Identifier",
                value: "RAG");
        }
    }
}
