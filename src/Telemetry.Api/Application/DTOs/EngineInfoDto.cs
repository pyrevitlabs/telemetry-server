using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Telemetry.Api.Application.DTOs
{
    /// <summary>
    ///     Script engine information.
    /// </summary>
    public class EngineInfoDto
    {
        /// <summary>
        ///     Engine type name.
        ///     <br /><a href="https://pyrevitlabs.notion.site/Engines-7973ca3328c34fd1a95462f5c655475b">Engine types</a>.
        /// </summary>
        [MaxLength(100)]
        [JsonPropertyName("type")]
        public required string Type { get; init; }

        /// <summary>
        ///     Engine version.
        ///     <br /> <a href="https://pyrevitlabs.notion.site/Engines-7973ca3328c34fd1a95462f5c655475b">Engines list</a>.
        /// </summary>
        [MaxLength(100)]
        [JsonPropertyName("version")]
        public required string Version { get; init; }

        /// <summary>
        ///     System paths using by script.
        /// </summary>
        [JsonPropertyName("syspath")]
        public string[]? SysPath { get; init; }

        /// <summary>
        ///     Dynamic script configs data.
        /// </summary>
        [MaxLength(8000)]
        [JsonPropertyName("configs")]
        public string? Configs { get; init; }
    }
}