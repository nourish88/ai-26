using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AdminBackend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class filemangementadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileStores",
                schema: "jugaai",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<string>(type: "text", nullable: true),
                    Identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Uri = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    CreatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileStores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileTypes",
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
                    table.PrimaryKey("PK_FileTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IngestionStatusTypes",
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
                    table.PrimaryKey("PK_IngestionStatusTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationFileStores",
                schema: "jugaai",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<string>(type: "text", nullable: true),
                    ApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    FileStoreId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationFileStores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationFileStores_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "jugaai",
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationFileStores_FileStores_FileStoreId",
                        column: x => x.FileStoreId,
                        principalSchema: "jugaai",
                        principalTable: "FileStores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                schema: "jugaai",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FileExtension = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    FileStoreId = table.Column<long>(type: "bigint", nullable: false),
                    FileStoreIdentifier = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UploadApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    IngestionStatusTypeId = table.Column<long>(type: "bigint", nullable: false),
                    FileTypeId = table.Column<long>(type: "bigint", nullable: false),
                    ErrorDetail = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedByUserCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_FileStores_FileStoreId",
                        column: x => x.FileStoreId,
                        principalSchema: "jugaai",
                        principalTable: "FileStores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Files_FileTypes_FileTypeId",
                        column: x => x.FileTypeId,
                        principalSchema: "jugaai",
                        principalTable: "FileTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Files_IngestionStatusTypes_IngestionStatusTypeId",
                        column: x => x.IngestionStatusTypeId,
                        principalSchema: "jugaai",
                        principalTable: "IngestionStatusTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "FileTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "Identifier", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate" },
                values: new object[,]
                {
                    { 1L, null, null, null, null, "personal", null, null, null, null },
                    { 2L, null, null, null, null, "application", null, null, null, null }
                });

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "IngestionStatusTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "Identifier", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate" },
                values: new object[,]
                {
                    { 1L, null, null, null, null, "extracting", null, null, null, null },
                    { 2L, null, null, null, null, "chunking", null, null, null, null },
                    { 3L, null, null, null, null, "indexing", null, null, null, null },
                    { 4L, null, null, null, null, "processed", null, null, null, null },
                    { 5L, null, null, null, null, "deleting", null, null, null, null },
                    { 6L, null, null, null, null, "error", null, null, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationFileStores_ApplicationId",
                schema: "jugaai",
                table: "ApplicationFileStores",
                column: "ApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationFileStores_ApplicationId_FileStoreId",
                schema: "jugaai",
                table: "ApplicationFileStores",
                columns: new[] { "ApplicationId", "FileStoreId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationFileStores_FileStoreId",
                schema: "jugaai",
                table: "ApplicationFileStores",
                column: "FileStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_FileStoreId",
                schema: "jugaai",
                table: "Files",
                column: "FileStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_FileTypeId",
                schema: "jugaai",
                table: "Files",
                column: "FileTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_IngestionStatusTypeId",
                schema: "jugaai",
                table: "Files",
                column: "IngestionStatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FileStores_Identifier",
                schema: "jugaai",
                table: "FileStores",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileTypes_Identifier",
                schema: "jugaai",
                table: "FileTypes",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IngestionStatusTypes_Identifier",
                schema: "jugaai",
                table: "IngestionStatusTypes",
                column: "Identifier",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationFileStores",
                schema: "jugaai");

            migrationBuilder.DropTable(
                name: "Files",
                schema: "jugaai");

            migrationBuilder.DropTable(
                name: "FileStores",
                schema: "jugaai");

            migrationBuilder.DropTable(
                name: "FileTypes",
                schema: "jugaai");

            migrationBuilder.DropTable(
                name: "IngestionStatusTypes",
                schema: "jugaai");
        }
    }
}
