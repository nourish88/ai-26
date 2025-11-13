using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminBackend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class mcpserverinitialdatafix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "McpServers",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Uri",
                value: "http://localhost:5164/api/mcp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "McpServers",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Uri",
                value: "http://localhost:5164");
        }
    }
}
