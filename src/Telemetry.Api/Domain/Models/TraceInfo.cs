using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Telemetry.Api.Domain.Constants;

namespace Telemetry.Api.Domain.Models
{
    /// <summary>
    ///     Script executed information.
    /// </summary>
    public class TraceInfo
    {
        /// <summary>
        ///     Script executed
        ///     <a href="https://www.revitapidocs.com/2022/ab42c8d3-d361-88d2-5043-2d427d1238fc.htm">message</a>.
        /// </summary>
        [MaxLength(8000)]
        [Column(PropertyNames.TraceMessage)]
        public required string Message { get; init; }

        /// <summary>
        ///     Script engine information.
        /// </summary>
        [Column(PropertyNames.Engine)]
        public required EngineInfo Engine { get; init; }
    }
}