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
                name: "events",
                columns: table => new
                {
                    _id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    handler_id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    type = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    timestamp = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    username = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    host_user = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    revitbuild = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    revit = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    cancelled = table.Column<bool>(type: "BOOLEAN", nullable: true),
                    cancellable = table.Column<bool>(type: "BOOLEAN", nullable: true),
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
                    table.PrimaryKey("PK_events", x => x._id);
                });

            migrationBuilder.CreateTable(
                name: "scripts",
                columns: table => new
                {
                    _id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    sessionid = table.Column<Guid>(type: "RAW(16)", nullable: false),
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
                    message = table.Column<string>(type: "NCLOB", maxLength: 8000, nullable: false),
                    type = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    version = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    syspath = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    configs = table.Column<string>(type: "json", maxLength: 8000, nullable: true),
                    commandresults = table.Column<string>(type: "NCLOB", maxLength: 8000, nullable: true)
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
