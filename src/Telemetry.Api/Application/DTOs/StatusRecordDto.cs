using System.Text.Json.Serialization;

namespace Telemetry.Api.Application.DTOs
{
    /// <summary>
    ///     Server status information.
    /// </summary>
    public class StatusRecordDto
    {
        /// <summary>
        ///     Connection status (pass or fail).
        /// </summary>
        [JsonPropertyName("status")]
        public required string Status { get; init; }

        /// <summary>
        ///     Service unique id.
        /// </summary>
        [JsonPropertyName("serviceid")]
        public required Guid ServiceId { get; init; }

        /// <summary>
        ///     Service version.
        /// </summary>
        [JsonPropertyName("version")]
        public required string Version { get; init; }

        /// <summary>
        ///     Check information.
        /// </summary>
        [JsonPropertyName("checks")]
        public required Dictionary<string, StatusCheckDto> Checks { get; init; }
    }

    /// <summary>
    /// Data transfer object representing the status check details.
    /// </summary>
    public class StatusCheckDto
    {
        /// <summary>
        ///     Data base connection status (pass or fail).
        /// </summary>
        [JsonPropertyName("status")]
        public required string Status { get; init; }

        /// <summary>
        ///     Data base version.
        /// </summary>
        [JsonPropertyName("version")]
        public required string Version { get; init; }
    }
}