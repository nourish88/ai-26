using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AdminBackend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Refactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "FileTypes",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "FileTypes",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "IngestionStatusTypes",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "IngestionStatusTypes",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "IngestionStatusTypes",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "IngestionStatusTypes",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "IngestionStatusTypes",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "IngestionStatusTypes",
                keyColumn: "Id",
                keyValue: 6L);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "jugaai",
                table: "IngestionStatusTypes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "jugaai",
                table: "FileTypes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "jugaai",
                table: "Files",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "IngestionStatusTypeId",
                schema: "jugaai",
                table: "Files",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "FileTypeId",
                schema: "jugaai",
                table: "Files",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "ApplicationSearchEngines",
                keyColumn: "Id",
                keyValue: 1L,
                column: "IndexName",
                value: "test-app-index");

            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "Embeddings",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Url",
                value: "http://127.0.0.1:4000");

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "FileStores",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "Identifier", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate", "Uri" },
                values: new object[] { 1L, null, null, null, null, "s3-local", null, null, null, null, "127.0.0.1:8090" });

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "FileTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "Identifier", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, null, null, null, null, "Personal", null, null, null, null },
                    { 2, null, null, null, null, "Application", null, null, null, null }
                });

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "IngestionStatusTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "Identifier", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, null, null, null, null, "ProcessingRequested", null, null, null, null },
                    { 2, null, null, null, null, "Extracting", null, null, null, null },
                    { 3, null, null, null, null, "Chunking", null, null, null, null },
                    { 4, null, null, null, null, "Indexing", null, null, null, null },
                    { 5, null, null, null, null, "Processed", null, null, null, null },
                    { 6, null, null, null, null, "DeletingRequested", null, null, null, null },
                    { 7, null, null, null, null, "ProcessingFailed", null, null, null, null }
                });

            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "SearchEngines",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Url",
                value: "http://127.0.0.1:9200");

            migrationBuilder.InsertData(
                schema: "jugaai",
                table: "ApplicationFileStores",
                columns: new[] { "Id", "ApplicationId", "CreatedAt", "CreatedBy", "CreatedByUserCode", "CreatedDate", "FileStoreId", "UpdatedAt", "UpdatedBy", "UpdatedByUserCode", "UpdatedDate" },
                values: new object[] { 1L, 1L, null, null, null, null, 1L, null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "ApplicationFileStores",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "FileTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "FileTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "IngestionStatusTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "IngestionStatusTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "IngestionStatusTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "IngestionStatusTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "IngestionStatusTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "IngestionStatusTypes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "IngestionStatusTypes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                schema: "jugaai",
                table: "FileStores",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "jugaai",
                table: "IngestionStatusTypes",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "jugaai",
                table: "FileTypes",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "jugaai",
                table: "Files",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "IngestionStatusTypeId",
                schema: "jugaai",
                table: "Files",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "FileTypeId",
                schema: "jugaai",
                table: "Files",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "ApplicationSearchEngines",
                keyColumn: "Id",
                keyValue: 1L,
                column: "IndexName",
                value: "test app index");

            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "Embeddings",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Url",
                value: "127.0.0.1:4000");

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

            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "SearchEngines",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Url",
                value: "127.0.0.1:9200");
        }
    }
}
