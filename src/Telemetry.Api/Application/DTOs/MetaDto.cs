using System.Text.Json.Serialization;

namespace Telemetry.Api.Application.DTOs
{
    /// <summary>
    ///     Record metadata.
    /// </summary>
    public class MetaDto
    {
        /// <summary>
        ///     Schema version.
        /// </summary>
        [JsonPropertyName("schema")]
        public required Version SchemaVersion { get; init; }
    }
}