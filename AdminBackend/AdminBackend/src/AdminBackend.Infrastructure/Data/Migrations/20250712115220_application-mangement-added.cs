using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AdminBackend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class applicationmangementadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationEmbeddings",
                schema: "jugaai",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<string>(type: "text", nullable: true),
                    ApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    EmbeddingId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationEmbeddings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationEmbeddings_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "jugaai",
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationEmbeddings_Embeddings_EmbeddingId",
                        column: x => x.EmbeddingId,
                        principalSchema: "jugaai",
                        principalTable: "Embeddings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationLlms",
                schema: "jugaai",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<string>(type: "text", nullable: true),
                    ApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    LlmId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationLlms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationLlms_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "jugaai",
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationLlms_Llms_LlmId",
                        column: x => x.LlmId,
                        principalSchema: "jugaai",
                        principalTable: "Llms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationEmbeddings_ApplicationId",
                schema: "jugaai",
                table: "ApplicationEmbeddings",
                column: "ApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationEmbeddings_ApplicationId_EmbeddingId",
                schema: "jugaai",
                table: "ApplicationEmbeddings",
                columns: new[] { "ApplicationId", "EmbeddingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationEmbeddings_EmbeddingId",
                schema: "jugaai",
                table: "ApplicationEmbeddings",
                column: "EmbeddingId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationLlms_ApplicationId",
                schema: "jugaai",
                table: "ApplicationLlms",
                column: "ApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationLlms_ApplicationId_LlmId",
                schema: "jugaai",
                table: "ApplicationLlms",
                columns: new[] { "ApplicationId", "LlmId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationLlms_LlmId",
                schema: "jugaai",
                table: "ApplicationLlms",
                column: "LlmId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationEmbeddings",
                schema: "jugaai");

            migrationBuilder.DropTable(
                name: "ApplicationLlms",
                schema: "jugaai");
        }
    }
}
