using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Telemetry.Api.Domain.Constants;

namespace Telemetry.Api.Domain.Models
{
    /// <summary>
    ///     Log record information.
    /// </summary>
    [Table("logs")]
    public class LogRecord
    {
        /// <summary>
        /// Log id.
        /// </summary>
        [Key]
        [Column(PropertyNames.Id)]
        public Guid Id { get; init; }

        /// <summary>
        ///     The time at which the event occurred.
        /// </summary>
        [Column(PropertyNames.Timestamp)]
        public DateTimeOffset Timestamp { get; init; }

        /// <summary>
        ///     The level of the event.
        /// </summary>
        [MaxLength(100)]
        [Column(PropertyNames.LogLevel)]
        public required string Level { get; init; }

        /// <summary>
        ///     The message template describing the event.
        /// </summary>
        [MaxLength(8000)]
        [Column(PropertyNames.MessageTemplate)]
        public required string MessageTemplate { get; init; }

        /// <summary>
        ///     The render message describing the event.
        /// </summary>
        [MaxLength(8000)]
        [Column(PropertyNames.RenderedMessage)]
        public required string RenderedMessage { get; init; }

        /// <summary>
        ///     An exception associated with the event, or null.
        /// </summary>
        [MaxLength(8000)]
        [Column(PropertyNames.Exception)]
        public string? Exception { get; init; }

        /// <summary>
        ///     Revit Session Id (unique revit start instance)
        /// </summary>
        [Column(PropertyNames.LogSessionId)]
        public Guid SessionId { get; init; }

        /// <summary>
        ///     Revit plugin name.
        /// </summary>
        [MaxLength(100)]
        [Column(PropertyNames.PluginName)]
        public required string PluginName { get; init; }

        /// <summary>
        ///     Revit Plugin Session Id  (unique revit plugin start instance)
        /// </summary>
        [Column(PropertyNames.PluginSessionId)]
        public Guid PluginSessionId { get; init; }

        /// <summary>
        ///     Environment UserName
        /// </summary>
        [MaxLength(100)]
        [Column(PropertyNames.EnvironmentUserName)]
        public required string EnvironmentUserName { get; init; }

        /// <summary>
        ///     Environment MachineName
        /// </summary>
        [MaxLength(100)]
        [Column(PropertyNames.EnvironmentMachineName)]
        public required string EnvironmentMachineName { get; init; }

        /// <summary>
        ///     Revit VersionBuild property.
        /// </summary>
        [MaxLength(100)]
        [Column(PropertyNames.LogRevitBuild)]
        public required string RevitBuild { get; init; }

        /// <summary>
        ///     Revit VersionNumber property.
        /// </summary>
        [Column(PropertyNames.LogRevitVersion)]
        public int RevitVersion { get; init; }

        /// <summary>
        ///     Revit Language property.
        /// </summary>
        [MaxLength(100)]
        [Column(PropertyNames.RevitLanguage)]
        public required string RevitLanguage { get; init; }

        /// <summary>
        ///     Revit Username property.
        /// </summary>
        [MaxLength(100)]
        [Column(PropertyNames.RevitUserName)]
        public string? RevitUserName { get; init; }

        /// <summary>
        ///     Revit Document.Title property.
        /// </summary>
        [MaxLength(250)]
        [Column(PropertyNames.RevitDocumentTitle)]
        public string? RevitDocumentTitle { get; init; }

        /// <summary>
        ///     Revit Document.PathName property.
        /// </summary>
        [MaxLength(1024)]
        [Column(PropertyNames.RevitDocumentPathName)]
        public string? RevitDocumentPathName { get; init; }

        /// <summary>
        ///     Revit ModelPath property.
        /// </summary>
        [MaxLength(1024)]
        [Column(PropertyNames.RevitDocumentModelPath)]
        public string? RevitDocumentModelPath { get; init; }

        /// <summary>
        ///     Dynamic properties data.
        /// </summary>
        [MaxLength(8000)]
        [Column(PropertyNames.LogEvent)]
        public string? LogEvent { get; init; }
    }
}
