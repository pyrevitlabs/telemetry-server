using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telemetry.Migrations.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class AddCancellableSqlite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Cancellable",
                table: "event_record",
                newName: "cancellable");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "cancellable",
                table: "event_record",
                newName: "Cancellable");
        }
    }
}
