using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AdminBackend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class chunkingstrategyadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChunkingStrategies",
                schema: "jugaai",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<string>(type: "text", nullable: true),
                    Identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsChunkingSizeRequired = table.Column<bool>(type: "boolean", nullable: false),
                    IsOverlapRequired = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChunkingStrategies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                schema: "jugaai",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    HasApplicationFile = table.Column<bool>(type: "boolean", nullable: false),
                    HasUserFile = table.Column<bool>(type: "boolean", nullable: false),
                    ChunkingStrategyId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applications_ChunkingStrategies_ChunkingStrategyId",
                        column: x => x.ChunkingStrategyId,
                        principalSchema: "jugaai",
                        principalTable: "ChunkingStrategies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationChunkingStrategies",
                schema: "jugaai",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<string>(type: "text", nullable: true),
                    ApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    ChunkingStrategyId = table.Column<long>(type: "bigint", nullable: false),
                    ChunkSize = table.Column<int>(type: "integer", nullable: true),
                    Overlap = table.Column<int>(type: "integer", nullable: true),
                    Seperator = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationChunkingStrategies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationChunkingStrategies_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "jugaai",
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationChunkingStrategies_ChunkingStrategies_ChunkingSt~",
                        column: x => x.ChunkingStrategyId,
                        principalSchema: "jugaai",
                        principalTable: "ChunkingStrategies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationChunkingStrategies_ApplicationId",
                schema: "jugaai",
                table: "ApplicationChunkingStrategies",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationChunkingStrategies_ChunkingStrategyId",
                schema: "jugaai",
                table: "ApplicationChunkingStrategies",
                column: "ChunkingStrategyId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ChunkingStrategyId",
                schema: "jugaai",
                table: "Applications",
                column: "ChunkingStrategyId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Identifier",
                schema: "jugaai",
                table: "Applications",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChunkingStrategies_Identifier",
                schema: "jugaai",
                table: "ChunkingStrategies",
                column: "Identifier",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationChunkingStrategies",
                schema: "jugaai");

            migrationBuilder.DropTable(
                name: "Applications",
                schema: "jugaai");

            migrationBuilder.DropTable(
                name: "ChunkingStrategies",
                schema: "jugaai");
        }
    }
}
