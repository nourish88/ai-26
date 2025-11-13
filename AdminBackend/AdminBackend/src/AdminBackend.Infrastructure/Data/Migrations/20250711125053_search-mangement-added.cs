using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AdminBackend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class searchmangementadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SearchEngineTypes",
                schema: "jugaai",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                    table.PrimaryKey("PK_SearchEngineTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchEngines",
                schema: "jugaai",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<string>(type: "text", nullable: true),
                    SearchEngineTypeId = table.Column<long>(type: "bigint", nullable: false),
                    Url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchEngines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SearchEngines_SearchEngineTypes_SearchEngineTypeId",
                        column: x => x.SearchEngineTypeId,
                        principalSchema: "jugaai",
                        principalTable: "SearchEngineTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationSearchEngines",
                schema: "jugaai",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<string>(type: "text", nullable: true),
                    ApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    SearchEngineId = table.Column<long>(type: "bigint", nullable: false),
                    IndexName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EmbeddingId = table.Column<long>(type: "bigint", nullable: false),
                    Identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    VectorSize = table.Column<int>(type: "integer", nullable: false),
                    CreatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationSearchEngines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationSearchEngines_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "jugaai",
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationSearchEngines_Embeddings_EmbeddingId",
                        column: x => x.EmbeddingId,
                        principalSchema: "jugaai",
                        principalTable: "Embeddings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationSearchEngines_SearchEngines_SearchEngineId",
                        column: x => x.SearchEngineId,
                        principalSchema: "jugaai",
                        principalTable: "SearchEngines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "SearchEngineTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "Identifier", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate" },
                values: new object[] { 1L, null, null, null, null, "elasticsearch", null, null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationSearchEngines_ApplicationId",
                schema: "jugaai",
                table: "ApplicationSearchEngines",
                column: "ApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationSearchEngines_EmbeddingId",
                schema: "jugaai",
                table: "ApplicationSearchEngines",
                column: "EmbeddingId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationSearchEngines_Identifier",
                schema: "jugaai",
                table: "ApplicationSearchEngines",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationSearchEngines_SearchEngineId",
                schema: "jugaai",
                table: "ApplicationSearchEngines",
                column: "SearchEngineId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchEngines_Identifier",
                schema: "jugaai",
                table: "SearchEngines",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SearchEngines_SearchEngineTypeId",
                schema: "jugaai",
                table: "SearchEngines",
                column: "SearchEngineTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchEngineTypes_Identifier",
                schema: "jugaai",
                table: "SearchEngineTypes",
                column: "Identifier",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationSearchEngines",
                schema: "jugaai");

            migrationBuilder.DropTable(
                name: "SearchEngines",
                schema: "jugaai");

            migrationBuilder.DropTable(
                name: "SearchEngineTypes",
                schema: "jugaai");
        }
    }
}
