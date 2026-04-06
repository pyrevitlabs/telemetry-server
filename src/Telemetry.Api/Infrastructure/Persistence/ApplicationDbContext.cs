using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MongoDB.Bson;
using MongoDB.EntityFrameworkCore.Extensions;
using System.Data;
using System.Data.Common;
using System.Text.Json;
using Telemetry.Api.Application.Interfaces;
using Telemetry.Api.Domain.Models;

namespace Telemetry.Api.Infrastructure.Persistence
{
    /// <summary>
    /// Represents the database context for the application, providing access to the database
    /// and handling the interactions between the application and the data storage.
    /// </summary>
    /// <remarks>
    /// This class is typically used with Entity Framework Core to define the database schema
    /// through DbSet properties and manage entity persistence.
    /// It encapsulates the data connection and performs CRUD operations on the underlying database.
    /// </remarks>
    /// <example>
    /// The <c>ApplicationDbContext</c> class should be instantiated and managed using dependency
    /// injection within the application's service container. Configure the DbContext options
    /// in the application startup or configuration class.
    /// </example>
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        /// <summary>
        /// Represents the database context for the application.
        /// Provides access to the database tables and enables CRUD operations.
        /// Extends from DbContext to leverage EF Core functionality for managing database interactions.
        /// </summary>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        /// <summary>
        /// Gets or sets the collection of event records associated with the current context.
        /// </summary>
        /// <remarks>
        /// This property holds a list of event data used for logging, auditing, or tracking purposes.
        /// It can be used to store and retrieve information about events in a structured format.
        /// </remarks>
        public DbSet<EventRecord> EventRecords => Set<EventRecord>();

        /// <summary>
        /// Gets or sets the collection of script records associated with the current context.
        /// </summary>
        /// <remarks>
        /// This property contains a list of script data, which can be used for processing,
        /// execution tracking, or storing metadata related to scripts.
        /// </remarks>
        public DbSet<ScriptRecord> ScriptRecords => Set<ScriptRecord>();

        /// <summary>
        /// Asynchronously checks whether a successful connection to the database can be established.
        /// This method verifies connectivity without performing any data manipulation or transactions.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a boolean value indicating
        /// whether the connection to the database was successful (true) or unsuccessful (false).
        /// </returns>
        public Task<bool> CanConnectAsync(CancellationToken cancellationToken)
        {
            return Database.CanConnectAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves the database provider being used for the application's data access.
        /// This method determines and returns the current database provider implementation.
        /// </summary>
        /// <returns>The database provider instance configured for the application.</returns>
        public string GetDbProvider()
        {
            return Database.ProviderName ?? "unknown";
        }

        /// <summary>
        /// Asynchronously retrieves the current version of the database.
        /// Useful for determining database schema versions or migrations required.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the database version as a string.</returns>
        public async Task<string> GetDbVersionAsync(CancellationToken cancellationToken)
        {
            try
            {
                string? query = null;
                if (Database.IsNpgsql())
                {
                    query = "SELECT version()";
                }
                else if (Database.IsOracle())
                {
                    query = "SELECT * FROM v$version;";
                }
                else if (Database.IsSqlServer())
                {
                    query = "SELECT @@VERSION";
                }
                else if (Database.IsSqlite())
                {
                    query = "SELECT sqlite_version()";
                }
                else if (Database.IsInMemory())
                {
                    return "memorydb";
                }

                if (query != null)
                {
                    await using DbCommand command = Database.GetDbConnection().CreateCommand();

                    command.CommandText = query;
                    if (command.Connection!.State != ConnectionState.Open)
                    {
                        await command.Connection.OpenAsync(cancellationToken);
                    }

                    object? result = await command.ExecuteScalarAsync(cancellationToken);
                    return result?.ToString() ?? "unknown";
                }
            }
            catch
            {
                // ignored
            }

            return "unknown";
        }

        /// <summary>
        /// Configures the model relationships, constraints, and schema definitions for the database context.
        /// Called when the database model is being created or initialized.
        /// </summary>
        /// <param name="modelBuilder">The builder used to define the model structure and relationships.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScriptRecord>(entity =>
            {
                entity.ToTable("scripts");
                entity.ToCollection("scripts");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("_id")
                    .HasElementName("_id");

                entity.Property(e => e.SessionId)
                    .HasColumnName("sessionid")
                    .HasElementName("sessionid");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasElementName("timestamp")
                    .HasBsonRepresentation(BsonType.DateTime);

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasElementName("username");

                entity.Property(e => e.HostUsername)
                    .HasColumnName("host_user")
                    .HasElementName("host_user");

                entity.Property(e => e.RevitBuild)
                    .HasColumnName("revitbuild")
                    .HasElementName("revitbuild");

                entity.Property(e => e.RevitVersion)
                    .HasColumnName("revit")
                    .HasElementName("revit");

                entity.Property(e => e.PyRevitVersion)
                    .HasColumnName("pyrevit")
                    .HasElementName("pyrevit");

                entity.Property(e => e.CloneName)
                    .HasColumnName("clone")
                    .HasElementName("clone");

                entity.Property(e => e.IsDebug)
                    .HasColumnName("debug")
                    .HasElementName("debug");

                entity.Property(e => e.IsConfig)
                    .HasColumnName("config")
                    .HasElementName("config");

                entity.Property(e => e.IsExecFromGui)
                    .HasColumnName("from_gui")
                    .HasElementName("from_gui");

                entity.Property(e => e.ExecId)
                    .HasColumnName("exec_id")
                    .HasElementName("exec_id");

                entity.Property(e => e.ExecTimestamp)
                    .HasColumnName("exec_timestamp")
                    .HasElementName("exec_timestamp")
                    .HasBsonRepresentation(BsonType.DateTime);

                entity.Property(e => e.CommandBundle)
                    .HasColumnName("commandbundle")
                    .HasElementName("commandbundle");

                entity.Property(e => e.CommandExtension)
                    .HasColumnName("commandextension")
                    .HasElementName("commandextension");

                entity.Property(e => e.CommandName)
                    .HasColumnName("commandname")
                    .HasElementName("commandname");

                entity.Property(e => e.CommandUniqueName)
                    .HasColumnName("commanduniquename")
                    .HasElementName("commanduniquename");

                entity.Property(e => e.DocumentName).HasColumnName("docname")
                    .HasElementName("docname");

                entity.Property(e => e.DocumentPath)
                    .HasColumnName("docpath")
                    .HasElementName("docpath");

                entity.Property(e => e.ResultCode)
                    .HasColumnName("resultcode")
                    .HasElementName("resultcode");

                entity.Property(e => e.ScriptPath)
                    .HasColumnName("scriptpath")
                    .HasElementName("scriptpath");

                entity.Property(e => e.CommandResults)
                    .HasColumnName("commandresults")
                    .HasElementName("commandresults");

                entity.Property(e => e.Meta)
                    .HasColumnName("meta")
                    .HasElementName("meta")
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, JsonSerializerOptions.Web),
                        v => JsonSerializer.Deserialize<MetaRecord>(v, JsonSerializerOptions.Web)!);

                entity.Property(e => e.Trace)
                    .HasColumnName("trace")
                    .HasElementName("trace")
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, JsonSerializerOptions.Web),
                        v => JsonSerializer.Deserialize<TraceInfo>(v, JsonSerializerOptions.Web)!);
            });

            modelBuilder.Entity<EventRecord>(entity =>
            {
                entity.ToTable("events");
                entity.ToCollection("events");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("_id")
                    .HasElementName("_id");

                entity.Property(e => e.HandlerId)
                    .HasColumnName("handler_id")
                    .HasElementName("handler_id");

                entity.Property(e => e.EventType)
                    .HasColumnName("type")
                    .HasElementName("type");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasElementName("status");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasElementName("timestamp")
                    .HasBsonRepresentation(BsonType.DateTime);

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasElementName("username");

                entity.Property(e => e.HostUsername)
                    .HasColumnName("host_user")
                    .HasElementName("host_user");

                entity.Property(e => e.RevitBuild)
                    .HasColumnName("revitbuild")
                    .HasElementName("revitbuild");

                entity.Property(e => e.RevitVersion)
                    .HasColumnName("revit")
                    .HasElementName("revit");

                entity.Property(e => e.Cancelled)
                    .HasColumnName("cancelled")
                    .HasElementName("cancelled");

                entity.Property(e => e.Cancellable)
                    .HasColumnName("cancellable")
                    .HasElementName("cancellable");

                entity.Property(e => e.DocumentId)
                    .HasColumnName("docid")
                    .HasElementName("docid");

                entity.Property(e => e.DocumentType)
                    .HasColumnName("doctype")
                    .HasElementName("doctype");

                entity.Property(e => e.DocumentTemplate)
                    .HasColumnName("doctemplate")
                    .HasElementName("doctemplate");

                entity.Property(e => e.DocumentName)
                    .HasColumnName("docname")
                    .HasElementName("docname");

                entity.Property(e => e.DocumentPath)
                    .HasColumnName("docpath")
                    .HasElementName("docpath");

                entity.Property(e => e.ProjectName)
                    .HasColumnName("projectname")
                    .HasElementName("projectname");

                entity.Property(e => e.ProjectNum)
                    .HasColumnName("projectnum")
                    .HasElementName("projectnum");

                entity.Property(e => e.EventArgs)
                    .HasColumnName("args")
                    .HasElementName("args");

                entity.Property(e => e.Meta)
                    .HasColumnName("meta")
                    .HasElementName("meta")
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, JsonSerializerOptions.Web),
                        v => JsonSerializer.Deserialize<MetaRecord>(v, JsonSerializerOptions.Web)!);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}