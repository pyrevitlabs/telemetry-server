using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Telemetry.Api.Domain.Models
{
    /// <summary>
    ///     Record metadata.
    /// </summary>
    public class MetaRecord
    {
        /// <summary>
        ///     Schema version.
        /// </summary>
        [JsonPropertyName("version")]
        public required Version SchemaVersion { get; init; }
    }
}