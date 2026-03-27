using Microsoft.EntityFrameworkCore;
using Telemetry.Api.Domain.Models;

namespace Telemetry.Api.Application.Interfaces
{
    /// <summary>
    /// Represents the contract for the application's database context.
    /// Defines the operations and entities available for data access in the application.
    /// </summary>
    public interface IApplicationDbContext
    {
        /// <summary>
        /// Represents a collection of event records associated with an entity or process.
        /// This property is used to store and manage information about specific occurrences
        /// or actions that are relevant to the application context.
        /// </summary>
        /// <remarks>
        /// Event records stored in this property can include details such as timestamps,
        /// descriptions, identifiers, and metadata. It is typically used for logging,
        /// auditing, or tracking purposes.
        /// </remarks>
        DbSet<EventRecord> EventRecords { get; }

        /// <summary>
        /// Stores a collection of script records associated with the application or system context.
        /// This property is utilized to manage and organize information related to scripts,
        /// their configurations, or execution details.
        /// </summary>
        /// <remarks>
        /// Script records contained in this property may include data such as script names,
        /// execution statuses, parameters, or any related metadata. It is commonly used
        /// for monitoring, debugging, or storing script-related information in a structured manner.
        /// </remarks>
        DbSet<ScriptRecord> ScriptRecords { get; }

        /// <summary>
        /// Asynchronously saves all changes made in the current context to the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation.
        /// The task result contains the number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously checks if a connection to the database can be established.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a boolean value indicating whether the connection attempt succeeded.</returns>
        Task<bool> CanConnectAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the database provider being used by the application.
        /// </summary>
        /// <returns>A string representing the name of the database provider.</returns>
        string GetDbProvider();

        /// <summary>
        /// Asynchronously retrieves the database version.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the version string of the database.</returns>
        Task<string> GetDbVersionAsync(CancellationToken cancellationToken);
    }
}