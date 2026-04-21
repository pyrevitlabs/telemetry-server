using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MongoDB.Bson;
using MongoDB.EntityFrameworkCore.Extensions;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text.Json;
using Telemetry.Api.Application.Interfaces;
using Telemetry.Api.Domain.Constants;
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
        /// Asynchronously adds a new event record to the database context.
        /// </summary>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        public async Task AddEventRecord(EventRecord eventRecord, CancellationToken cancellationToken)
        {
            await EventRecords.AddAsync(eventRecord, cancellationToken);
        }

        /// <summary>
        /// Asynchronously adds a new script record to the database context.
        /// </summary>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        public async Task AddScriptRecord(ScriptRecord scriptRecord, CancellationToken cancellationToken)
        {
            await ScriptRecords.AddAsync(scriptRecord, cancellationToken);
        }

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
                else if (Database.ProviderName!.Equals("MongoDB.EntityFrameworkCore"))
                {
                    return $"EF Core lib version: " +
                           $"{typeof(MongoPropertyBuilderExtensions).Assembly.GetName().Version?.ToString() ?? "mongodb"}";
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

        private static void ConfigureMongoDbModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScriptRecord>(entity =>
            {
                entity.Property(e => e.Timestamp)
                    .HasBsonRepresentation(BsonType.DateTime);
                entity.Property(e => e.ExecTimestamp)
                    .HasBsonRepresentation(BsonType.DateTime);
            });

            modelBuilder.Entity<EventRecord>(entity =>
            {
                entity.Property(e => e.Timestamp)
                    .HasBsonRepresentation(BsonType.DateTime);
            });
        }

        private void ConfigureSqlModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScriptRecord>(entity =>
            {
                entity.OwnsOne(e => e.Trace, trace =>
                {
                    trace.Property(e => e.Message).HasColumnName(PropertyNames.TraceMessage);

                    trace.OwnsOne(e => e.Engine, engine =>
                    {
                        if (Database.IsOracle())
                        {
                            engine.Property(e => e.Configs)
                                .HasColumnName(PropertyNames.EngineConfigs)
                                .HasColumnType("json");
                        }
                        else if (Database.IsNpgsql())
                        {
                            engine.Property(e => e.Configs)
                                .HasColumnName(PropertyNames.EngineConfigs)
                                .HasColumnType("jsonb");
                        }

                        engine.Property(e => e.Type).HasColumnName(PropertyNames.EngineType);
                        engine.Property(e => e.Version).HasColumnName(PropertyNames.EngineVersion);
                        engine.Property(e => e.SysPaths).HasColumnName(PropertyNames.EngineSysPaths);
                    });
                });
            });
        }
    }
}