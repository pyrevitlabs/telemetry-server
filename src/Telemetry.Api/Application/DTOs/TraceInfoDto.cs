using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Telemetry.Api.Application.DTOs
{
    /// <summary>
    ///     Script executed information.
    /// </summary>
    public class TraceInfoDto
    {
        /// <summary>
        ///     Script executed
        ///     <a href="https://www.revitapidocs.com/2022/ab42c8d3-d361-88d2-5043-2d427d1238fc.htm">message</a>.
        /// </summary>
        [MaxLength(8000)]
        [JsonPropertyName("message")]
        public required string Message { get; init; }

        /// <summary>
        ///     Script engine information.
        /// </summary>
        [JsonPropertyName("engine")]
        public required EngineInfoDto Engine { get; init; }
    }
}