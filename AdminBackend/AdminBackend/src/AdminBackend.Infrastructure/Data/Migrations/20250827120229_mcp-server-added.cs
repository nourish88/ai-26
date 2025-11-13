using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AdminBackend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class mcpserveradded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "McpServers",
                schema: "jugaai",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<string>(type: "text", nullable: true),
                    Identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Uri = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_McpServers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationMcpServers",
                schema: "jugaai",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<string>(type: "text", nullable: true),
                    ApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    McpServerId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationMcpServers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationMcpServers_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "jugaai",
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationMcpServers_McpServers_McpServerId",
                        column: x => x.McpServerId,
                        principalSchema: "jugaai",
                        principalTable: "McpServers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "McpServers",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "Identifier", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate", "Uri" },
                values: new object[] { 1L, null, null, null, null, "toolgateway", null, null, null, null, "http://localhost:5164" });

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "ApplicationMcpServers",
                columns: new[] { "Id", "ApplicationId", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "McpServerId", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate" },
                values: new object[] { 1L, 1L, null, null, null, null, 1L, null, null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationMcpServers_ApplicationId_McpServerId",
                schema: "jugaai",
                table: "ApplicationMcpServers",
                columns: new[] { "ApplicationId", "McpServerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationMcpServers_McpServerId",
                schema: "jugaai",
                table: "ApplicationMcpServers",
                column: "McpServerId");

            migrationBuilder.CreateIndex(
                name: "IX_McpServers_Identifier",
                schema: "jugaai",
                table: "McpServers",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_McpServers_Uri",
                schema: "jugaai",
                table: "McpServers",
                column: "Uri",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationMcpServers",
                schema: "jugaai");

            migrationBuilder.DropTable(
                name: "McpServers",
                schema: "jugaai");
        }
    }
}
