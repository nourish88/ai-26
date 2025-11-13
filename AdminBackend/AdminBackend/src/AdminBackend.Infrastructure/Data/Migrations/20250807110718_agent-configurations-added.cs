using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AdminBackend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class agentconfigurationsadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ApplicationTypeId",
                schema: "jugaai",
                table: "Applications",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "jugaai",
                table: "Applications",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "MemoryTypeId",
                schema: "jugaai",
                table: "Applications",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "OutputTypeId",
                schema: "jugaai",
                table: "Applications",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "SystemPrompt",
                schema: "jugaai",
                table: "Applications",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "EnableThinking",
                schema: "jugaai",
                table: "ApplicationLlms",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<float>(
                name: "Temperature",
                schema: "jugaai",
                table: "ApplicationLlms",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "TopP",
                schema: "jugaai",
                table: "ApplicationLlms",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateTable(
                name: "ApplicationTypes",
                schema: "jugaai",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<string>(type: "text", nullable: true),
                    Identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MemoryTypes",
                schema: "jugaai",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<string>(type: "text", nullable: true),
                    Identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemoryTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutputTypes",
                schema: "jugaai",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<string>(type: "text", nullable: true),
                    Identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutputTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "ApplicationTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "Identifier", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate" },
                values: new object[,]
                {
                    { 1L, null, null, null, null, "REACT", null, null, null, null },
                    { 2L, null, null, null, null, "RAG", null, null, null, null },
                    { 3L, null, null, null, null, "AGENTIC_RAG", null, null, null, null },
                    { 4L, null, null, null, null, "REFLECTIVE_RAG", null, null, null, null }
                });

            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "Applications",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ApplicationTypeId", "Description", "MemoryTypeId", "OutputTypeId", "SystemPrompt" },
                values: new object[] { 1L, "This is a test application for the AI Orchestrator.", 2L, 3L, "You are a helpful AI assistant." });

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "MemoryTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "Identifier", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate" },
                values: new object[,]
                {
                    { 1L, null, null, null, null, "MEMORY", null, null, null, null },
                    { 2L, null, null, null, null, "MONGO", null, null, null, null },
                    { 3L, null, null, null, null, "POSTGRE", null, null, null, null }
                });

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "OutputTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "Identifier", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate" },
                values: new object[,]
                {
                    { 1L, null, null, null, null, "BLOCK", null, null, null, null },
                    { 2L, null, null, null, null, "STREAMING", null, null, null, null },
                    { 3L, null, null, null, null, "BOTH", null, null, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicationTypeId",
                schema: "jugaai",
                table: "Applications",
                column: "ApplicationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_MemoryTypeId",
                schema: "jugaai",
                table: "Applications",
                column: "MemoryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_OutputTypeId",
                schema: "jugaai",
                table: "Applications",
                column: "OutputTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationTypes_Identifier",
                schema: "jugaai",
                table: "ApplicationTypes",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemoryTypes_Identifier",
                schema: "jugaai",
                table: "MemoryTypes",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutputTypes_Identifier",
                schema: "jugaai",
                table: "OutputTypes",
                column: "Identifier",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_ApplicationTypes_ApplicationTypeId",
                schema: "jugaai",
                table: "Applications",
                column: "ApplicationTypeId",
                principalSchema: "jugaai",
                principalTable: "ApplicationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_MemoryTypes_MemoryTypeId",
                schema: "jugaai",
                table: "Applications",
                column: "MemoryTypeId",
                principalSchema: "jugaai",
                principalTable: "MemoryTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_OutputTypes_OutputTypeId",
                schema: "jugaai",
                table: "Applications",
                column: "OutputTypeId",
                principalSchema: "jugaai",
                principalTable: "OutputTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_ApplicationTypes_ApplicationTypeId",
                schema: "jugaai",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_MemoryTypes_MemoryTypeId",
                schema: "jugaai",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_OutputTypes_OutputTypeId",
                schema: "jugaai",
                table: "Applications");

            migrationBuilder.DropTable(
                name: "ApplicationTypes",
                schema: "jugaai");

            migrationBuilder.DropTable(
                name: "MemoryTypes",
                schema: "jugaai");

            migrationBuilder.DropTable(
                name: "OutputTypes",
                schema: "jugaai");

            migrationBuilder.DropIndex(
                name: "IX_Applications_ApplicationTypeId",
                schema: "jugaai",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_MemoryTypeId",
                schema: "jugaai",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_OutputTypeId",
                schema: "jugaai",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "ApplicationTypeId",
                schema: "jugaai",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "jugaai",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "MemoryTypeId",
                schema: "jugaai",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "OutputTypeId",
                schema: "jugaai",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "SystemPrompt",
                schema: "jugaai",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "EnableThinking",
                schema: "jugaai",
                table: "ApplicationLlms");

            migrationBuilder.DropColumn(
                name: "Temperature",
                schema: "jugaai",
                table: "ApplicationLlms");

            migrationBuilder.DropColumn(
                name: "TopP",
                schema: "jugaai",
                table: "ApplicationLlms");
        }
    }
}
