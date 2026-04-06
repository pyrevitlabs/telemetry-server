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
            if (Database.ProviderName!.Equals("MongoDB.EntityFrameworkCore"))
            {
                ConfigureMongoDbModel(modelBuilder);
            }
            else
            {
                ConfigureSqlModel(modelBuilder);
            }

            base.OnModelCreating(modelBuilder);
        }

        private void ConfigureMongoDbModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScriptRecord>(entity =>
            {
                entity.ToCollection(DbConstants.Tables.Scripts);
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasElementName(DbConstants.Columns.Id);
                entity.Property(e => e.SessionId).HasElementName(DbConstants.Columns.SessionId);

                entity.Property(e => e.Timestamp)
                    .HasElementName(DbConstants.Columns.Timestamp)
                    .HasBsonRepresentation(BsonType.DateTime);

                entity.Property(e => e.Username).HasElementName(DbConstants.Columns.Username);
                entity.Property(e => e.HostUsername).HasElementName(DbConstants.Columns.HostUsername);
                entity.Property(e => e.RevitBuild).HasElementName(DbConstants.Columns.RevitBuild);
                entity.Property(e => e.RevitVersion).HasElementName(DbConstants.Columns.RevitVersion);
                entity.Property(e => e.PyRevitVersion).HasElementName(DbConstants.Columns.PyRevitVersion);
                entity.Property(e => e.CloneName).HasElementName(DbConstants.Columns.CloneName);
                entity.Property(e => e.IsDebug).HasElementName(DbConstants.Columns.IsDebug);
                entity.Property(e => e.IsConfig).HasElementName(DbConstants.Columns.IsConfig);
                entity.Property(e => e.IsExecFromGui).HasElementName(DbConstants.Columns.IsExecFromGui);
                entity.Property(e => e.ExecId).HasElementName(DbConstants.Columns.ExecId);

                entity.Property(e => e.ExecTimestamp)
                    .HasElementName(DbConstants.Columns.ExecTimestamp)
                    .HasBsonRepresentation(BsonType.DateTime);

                entity.Property(e => e.CommandBundle).HasElementName(DbConstants.Columns.CommandBundle);
                entity.Property(e => e.CommandExtension).HasElementName(DbConstants.Columns.CommandExtension);
                entity.Property(e => e.CommandName).HasElementName(DbConstants.Columns.CommandName);
                entity.Property(e => e.CommandUniqueName).HasElementName(DbConstants.Columns.CommandUniqueName);
                entity.Property(e => e.DocumentName).HasElementName(DbConstants.Columns.DocumentName);
                entity.Property(e => e.DocumentPath).HasElementName(DbConstants.Columns.DocumentPath);
                entity.Property(e => e.ResultCode).HasElementName(DbConstants.Columns.ResultCode);
                entity.Property(e => e.ScriptPath).HasElementName(DbConstants.Columns.ScriptPath);
                entity.Property(e => e.CommandResults).HasElementName(DbConstants.Columns.CommandResults);

                entity.OwnsOne(e => e.Trace, trace =>
                {
                    trace.Property(e => e.Message).HasElementName(DbConstants.Columns.TraceMessage);
                    trace.Property(e => e.Engine.Type).HasElementName(DbConstants.Columns.EngineType);
                    trace.Property(e => e.Engine.Version).HasElementName(DbConstants.Columns.EngineVersion);
                    trace.Property(e => e.Engine.Configs).HasElementName(DbConstants.Columns.EngineConfigs);
                    trace.Property(e => e.Engine.SysPaths).HasElementName(DbConstants.Columns.EngineSysPaths);
                });
            });

            modelBuilder.Entity<EventRecord>(entity =>
            {
                entity.ToCollection(DbConstants.Tables.Events);
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasElementName(DbConstants.Columns.Id);
                entity.Property(e => e.HandlerId).HasElementName(DbConstants.Columns.HandlerId);
                entity.Property(e => e.EventType).HasElementName(DbConstants.Columns.EventType);
                entity.Property(e => e.Status).HasElementName(DbConstants.Columns.Status);

                entity.Property(e => e.Timestamp)
                    .HasElementName(DbConstants.Columns.Timestamp)
                    .HasBsonRepresentation(BsonType.DateTime);

                entity.Property(e => e.Username).HasElementName(DbConstants.Columns.Username);
                entity.Property(e => e.HostUsername).HasElementName(DbConstants.Columns.HostUsername);
                entity.Property(e => e.RevitBuild).HasElementName(DbConstants.Columns.RevitBuild);
                entity.Property(e => e.RevitVersion).HasElementName(DbConstants.Columns.RevitVersion);
                entity.Property(e => e.Cancelled).HasElementName(DbConstants.Columns.Cancelled);
                entity.Property(e => e.Cancellable).HasElementName(DbConstants.Columns.Cancellable);
                entity.Property(e => e.DocumentId).HasElementName(DbConstants.Columns.DocumentId);
                entity.Property(e => e.DocumentType).HasElementName(DbConstants.Columns.DocumentType);
                entity.Property(e => e.DocumentTemplate).HasElementName(DbConstants.Columns.DocumentTemplate);
                entity.Property(e => e.DocumentName).HasElementName(DbConstants.Columns.DocumentName);
                entity.Property(e => e.DocumentPath).HasElementName(DbConstants.Columns.DocumentPath);
                entity.Property(e => e.ProjectName).HasElementName(DbConstants.Columns.ProjectName);
                entity.Property(e => e.ProjectNum).HasElementName(DbConstants.Columns.ProjectNum);
                entity.Property(e => e.EventArgs).HasElementName(DbConstants.Columns.EventArgs);
            });
        }

        private void ConfigureSqlModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScriptRecord>(entity =>
            {
                entity.ToTable(DbConstants.Tables.Scripts);
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName(DbConstants.Columns.Id);
                entity.Property(e => e.SessionId).HasColumnName(DbConstants.Columns.SessionId);
                entity.Property(e => e.Timestamp).HasColumnName(DbConstants.Columns.Timestamp);
                entity.Property(e => e.Username).HasColumnName(DbConstants.Columns.Username);
                entity.Property(e => e.HostUsername).HasColumnName(DbConstants.Columns.HostUsername);
                entity.Property(e => e.RevitBuild).HasColumnName(DbConstants.Columns.RevitBuild);
                entity.Property(e => e.RevitVersion).HasColumnName(DbConstants.Columns.RevitVersion);
                entity.Property(e => e.PyRevitVersion).HasColumnName(DbConstants.Columns.PyRevitVersion);
                entity.Property(e => e.CloneName).HasColumnName(DbConstants.Columns.CloneName);
                entity.Property(e => e.IsDebug).HasColumnName(DbConstants.Columns.IsDebug);
                entity.Property(e => e.IsConfig).HasColumnName(DbConstants.Columns.IsConfig);
                entity.Property(e => e.IsExecFromGui).HasColumnName(DbConstants.Columns.IsExecFromGui);
                entity.Property(e => e.ExecId).HasColumnName(DbConstants.Columns.ExecId);
                entity.Property(e => e.ExecTimestamp).HasColumnName(DbConstants.Columns.ExecTimestamp);
                entity.Property(e => e.CommandBundle).HasColumnName(DbConstants.Columns.CommandBundle);
                entity.Property(e => e.CommandExtension).HasColumnName(DbConstants.Columns.CommandExtension);
                entity.Property(e => e.CommandName).HasColumnName(DbConstants.Columns.CommandName);
                entity.Property(e => e.CommandUniqueName).HasColumnName(DbConstants.Columns.CommandUniqueName);
                entity.Property(e => e.DocumentName).HasColumnName(DbConstants.Columns.DocumentName);
                entity.Property(e => e.DocumentPath).HasColumnName(DbConstants.Columns.DocumentPath);
                entity.Property(e => e.ResultCode).HasColumnName(DbConstants.Columns.ResultCode);
                entity.Property(e => e.ScriptPath).HasColumnName(DbConstants.Columns.ScriptPath);
                entity.Property(e => e.CommandResults).HasColumnName(DbConstants.Columns.CommandResults);

                entity.OwnsOne(e => e.Trace, trace =>
                {
                    trace.Property(e => e.Message).HasColumnName(DbConstants.Columns.TraceMessage);
                    trace.Property(e => e.Engine.Type).HasColumnName(DbConstants.Columns.EngineType);
                    trace.Property(e => e.Engine.Version).HasColumnName(DbConstants.Columns.EngineVersion);
                    trace.Property(e => e.Engine.Configs).HasColumnName(DbConstants.Columns.EngineConfigs);

                    trace.Property(e => e.Engine.SysPaths)
                        .HasColumnName(DbConstants.Columns.EngineSysPaths)
                        .HasConversion(
                            v => v == null ? null : string.Join(";", v),
                            v => string.IsNullOrEmpty(v) ? null : v.Split(";")
                        );
                });
            });

            modelBuilder.Entity<EventRecord>(entity =>
            {
                entity.ToTable(DbConstants.Tables.Events);
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName(DbConstants.Columns.Id);
                entity.Property(e => e.HandlerId).HasColumnName(DbConstants.Columns.HandlerId);
                entity.Property(e => e.EventType).HasColumnName(DbConstants.Columns.EventType);
                entity.Property(e => e.Status).HasColumnName(DbConstants.Columns.Status);
                entity.Property(e => e.Timestamp).HasColumnName(DbConstants.Columns.Timestamp);
                entity.Property(e => e.Username).HasColumnName(DbConstants.Columns.Username);
                entity.Property(e => e.HostUsername).HasColumnName(DbConstants.Columns.HostUsername);
                entity.Property(e => e.RevitBuild).HasColumnName(DbConstants.Columns.RevitBuild);
                entity.Property(e => e.RevitVersion).HasColumnName(DbConstants.Columns.RevitVersion);
                entity.Property(e => e.Cancelled).HasColumnName(DbConstants.Columns.Cancelled);
                entity.Property(e => e.Cancellable).HasColumnName(DbConstants.Columns.Cancellable);
                entity.Property(e => e.DocumentId).HasColumnName(DbConstants.Columns.DocumentId);
                entity.Property(e => e.DocumentType).HasColumnName(DbConstants.Columns.DocumentType);
                entity.Property(e => e.DocumentTemplate).HasColumnName(DbConstants.Columns.DocumentTemplate);
                entity.Property(e => e.DocumentName).HasColumnName(DbConstants.Columns.DocumentName);
                entity.Property(e => e.DocumentPath).HasColumnName(DbConstants.Columns.DocumentPath);
                entity.Property(e => e.ProjectName).HasColumnName(DbConstants.Columns.ProjectName);
                entity.Property(e => e.ProjectNum).HasColumnName(DbConstants.Columns.ProjectNum);
                entity.Property(e => e.EventArgs).HasColumnName(DbConstants.Columns.EventArgs);
            });
        }
    }
}