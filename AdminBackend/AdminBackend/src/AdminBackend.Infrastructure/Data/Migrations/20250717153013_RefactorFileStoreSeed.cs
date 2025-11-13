using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminBackend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorFileStoreSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "FileStores",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Identifier",
                value: "s3");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "jugaai",
                table: "FileStores",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Identifier",
                value: "s3-local");
        }
    }
}
