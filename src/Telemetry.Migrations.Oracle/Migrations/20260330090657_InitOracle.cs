using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telemetry.Migrations.Oracle.Migrations
{
    /// <inheritdoc />
    public partial class InitOracle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "event_record",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    handler_id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    meta = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    type = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    timestamp = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    username = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    host_user = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    revitbuild = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    revit = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    cancelled = table.Column<bool>(type: "BOOLEAN", nullable: true),
                    Cancellable = table.Column<bool>(type: "BOOLEAN", nullable: true),
                    docid = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    doctype = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    doctemplate = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    docname = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: true),
                    docpath = table.Column<string>(type: "NVARCHAR2(1024)", maxLength: 1024, nullable: true),
                    projectname = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: true),
                    projectnum = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    args = table.Column<string>(type: "NCLOB", maxLength: 8000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_record", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "script_record",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    sessionid = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    meta = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    timestamp = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    username = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    host_user = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    revitbuild = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    revit = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    pyrevit = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    clone = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    debug = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    config = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    from_gui = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    exec_id = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    exec_timestamp = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    commandbundle = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: false),
                    commandextension = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: false),
                    commandname = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: false),
                    commanduniquename = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    docname = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: true),
                    docpath = table.Column<string>(type: "NVARCHAR2(1024)", maxLength: 1024, nullable: true),
                    resultcode = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    scriptpath = table.Column<string>(type: "NVARCHAR2(1024)", maxLength: 1024, nullable: false),
                    trace = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    commandresults = table.Column<string>(type: "NCLOB", maxLength: 8000, nullable: true)
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
