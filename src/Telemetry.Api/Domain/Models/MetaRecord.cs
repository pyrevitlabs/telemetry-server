using MongoDB.Bson.Serialization.Attributes;

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
        [BsonElement("schema")]
        public required Version SchemaVersion { get; init; }
    }
}