using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AdminBackend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class extractionmangementadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExtractorEngineTypes",
                schema: "jugaai",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<string>(type: "text", nullable: true),
                    Identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Word = table.Column<bool>(type: "boolean", nullable: false),
                    Txt = table.Column<bool>(type: "boolean", nullable: false),
                    Pdf = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtractorEngineTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationExtractorEngines",
                schema: "jugaai",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<string>(type: "text", nullable: true),
                    ApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    ExtractorEngineTypeId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationExtractorEngines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationExtractorEngines_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "jugaai",
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationExtractorEngines_ExtractorEngineTypes_ExtractorE~",
                        column: x => x.ExtractorEngineTypeId,
                        principalSchema: "jugaai",
                        principalTable: "ExtractorEngineTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "ChunkingStrategies",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "Identifier", "IsChunkingSizeRequired", "IsOverlapRequired", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate" },
                values: new object[,]
                {
                    { 1L, null, null, null, null, "fixed-size", true, true, null, null, null, null },
                    { 2L, null, null, null, null, "html", true, true, null, null, null, null },
                    { 3L, null, null, null, null, "markdown", true, true, null, null, null, null }
                });

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "ExtractorEngineTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "Identifier", "Pdf", "Txt", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate", "Word" },
                values: new object[] { 1L, null, null, null, null, "markitdown", true, true, null, null, null, null, true });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationExtractorEngines_ApplicationId",
                schema: "jugaai",
                table: "ApplicationExtractorEngines",
                column: "ApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationExtractorEngines_ExtractorEngineTypeId",
                schema: "jugaai",
                table: "ApplicationExtractorEngines",
                column: "ExtractorEngineTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtractorEngineTypes_Identifier",
                schema: "jugaai",
                table: "ExtractorEngineTypes",
                column: "Identifier",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationExtractorEngines",
                schema: "jugaai");

            migrationBuilder.DropTable(
                name: "ExtractorEngineTypes",
                schema: "jugaai");

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "ChunkingStrategies",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "ChunkingStrategies",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "ChunkingStrategies",
                keyColumn: "Id",
                keyValue: 3L);
        }
    }
}
