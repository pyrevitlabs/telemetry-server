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
                name: "events",
                columns: table => new
                {
                    _id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    handler_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    host_user = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    revitbuild = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    revit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    cancelled = table.Column<bool>(type: "bit", nullable: true),
                    cancellable = table.Column<bool>(type: "bit", nullable: true),
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
                    table.PrimaryKey("PK_events", x => x._id);
                });

            migrationBuilder.CreateTable(
                name: "scripts",
                columns: table => new
                {
                    _id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sessionid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    message = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    version = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    syspath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    configs = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    commandresults = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scripts", x => x._id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "scripts");
        }
    }
}
