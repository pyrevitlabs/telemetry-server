using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telemetry.Migrations.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class InitSqlServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "event_record",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    handler_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    meta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    host_user = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    revitbuild = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    revit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    cancelled = table.Column<bool>(type: "bit", nullable: true),
                    Cancellable = table.Column<bool>(type: "bit", nullable: true),
                    docid = table.Column<int>(type: "int", nullable: false),
                    doctype = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    doctemplate = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    docname = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    docpath = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    projectname = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    projectnum = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    args = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_record", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "script_record",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sessionid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    meta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    host_user = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    revitbuild = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    revit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    pyrevit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    clone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    debug = table.Column<bool>(type: "bit", nullable: false),
                    config = table.Column<bool>(type: "bit", nullable: false),
                    from_gui = table.Column<bool>(type: "bit", nullable: false),
                    exec_id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    exec_timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    commandbundle = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    commandextension = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    commandname = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    commanduniquename = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    docname = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    docpath = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    resultcode = table.Column<int>(type: "int", nullable: false),
                    scriptpath = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    trace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    commandresults = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_script_record", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_record");

            migrationBuilder.DropTable(
                name: "script_record");
        }
    }
}
