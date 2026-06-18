using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Telemetry.Api.Domain.Constants;
using Telemetry.Api.JsonConverters;

namespace Telemetry.Api.Application.DTOs
{
    /// <summary>
    ///     Log event record class.
    /// </summary>
    public sealed class LogRecordDto
    {
        /// <summary>
        ///     The time at which the event occurred.
        /// </summary>
        [JsonPropertyName(PropertyNames.Timestamp)]
        public DateTimeOffset Timestamp { get; init; }

        /// <summary>
        ///     The level of the event.
        /// </summary>
        [MaxLength(100)]
        [JsonPropertyName(PropertyNames.LogLevel)]
        public required string Level { get; init; }

        /// <summary>
        ///     The message template describing the event.
        /// </summary>
        [MaxLength(8000)]
        [JsonPropertyName(PropertyNames.MessageTemplate)]
        public required string MessageTemplate { get; init; }

        /// <summary>
        ///     The render message describing the event.
        /// </summary>
        [MaxLength(8000)]
        [JsonPropertyName(PropertyNames.RenderedMessage)]
        public required string RenderedMessage { get; init; }

        /// <summary>
        ///     An exception associated with the event, or null.
        /// </summary>
        [MaxLength(8000)]
        [JsonPropertyName(PropertyNames.Exception)]
        [JsonConverter(typeof(DynamicDataJsonConverter))]
        public string? Exception { get; init; }

        /// <summary>
        ///     Revit Session Id (unique revit start instance)
        /// </summary>
        [JsonPropertyName(PropertyNames.LogSessionId)]
        public Guid SessionId { get; init; }

        /// <summary>
        ///     Revit plugin name.
        /// </summary>
        [MaxLength(100)]
        [JsonPropertyName(PropertyNames.PluginName)]
        public required string PluginName { get; init; }

        /// <summary>
        ///     Revit Plugin Session Id  (unique revit plugin start instance)
        /// </summary>
        [JsonPropertyName(PropertyNames.PluginSessionId)]
        public Guid PluginSessionId { get; init; }

        /// <summary>
        ///     Environment UserName
        /// </summary>
        [MaxLength(100)]
        [JsonPropertyName(PropertyNames.EnvironmentUserName)]
        public required string EnvironmentUserName { get; init; }

        /// <summary>
        ///     Environment MachineName
        /// </summary>
        [MaxLength(100)]
        [JsonPropertyName(PropertyNames.EnvironmentMachineName)]
        public required string EnvironmentMachineName { get; init; }

        /// <summary>
        ///     Revit <a href="https://www.revitapidocs.com/2017.1/04ef312a-e25a-cbcd-40c4-43fe6311e677.htm">VersionBuild</a>
        ///     property.
        /// </summary>
        [MaxLength(100)]
        [JsonPropertyName(PropertyNames.LogRevitBuild)]
        public required string RevitBuild { get; init; }

        /// <summary>
        ///     Revit <a href="https://www.revitapidocs.com/2017.1/320391bf-2c21-98ca-192c-da1d9becff11.htm">VersionNumber</a>
        ///     property.
        /// </summary>
        [JsonPropertyName(PropertyNames.LogRevitVersion)]
        public int RevitVersion { get; init; }

        /// <summary>
        ///     Revit <a href="https://www.revitapidocs.com/2017.1/2b1d8b80-a11c-2a57-63bd-6c0d67691879.htm">Language</a> property.
        /// </summary>
        [MaxLength(100)]
        [JsonPropertyName(PropertyNames.RevitLanguage)]
        public required string RevitLanguage { get; init; }

        /// <summary>
        ///     Revit <a href="https://www.revitapidocs.com/2017.1/2a7c8664-de0d-7a43-e670-2e733e579609.htm">Username</a> property.
        /// </summary>
        [MaxLength(100)]
        [JsonPropertyName(PropertyNames.RevitUserName)]
        public string? RevitUserName { get; init; }

        /// <summary>
        ///     Revit <a href="https://www.revitapidocs.com/2017.1/4cee7916-d799-fc83-daf3-97cb03900b72.htm">Document.Title</a>
        ///     property.
        /// </summary>
        [MaxLength(250)]
        [JsonPropertyName(PropertyNames.RevitDocumentTitle)]
        public string? RevitDocumentTitle { get; init; }

        /// <summary>
        ///     Revit <a href="https://www.revitapidocs.com/2022/8a92a6fd-ce25-cd86-2068-f9dcb24d72d6.htm">Document.PathName</a>
        ///     property.
        /// </summary>
        [MaxLength(1024)]
        [JsonPropertyName(PropertyNames.RevitDocumentPathName)]
        public string? RevitDocumentPathName { get; init; }

        /// <summary>
        ///     Revit ModelPath property.
        ///     <br />If
        ///     <a href="https://www.revitapidocs.com/2020/e12f7980-ba6c-2e72-6687-f0bf807ff014.htm">Document.IsModelInCloud</a> is true
        ///     <a href="https://www.revitapidocs.com/2020/087a7c14-1a6e-7022-c47b-923e90f4c5be.htm">Document.GetCloudModelPath()</a>.
        ///     <br />If
        ///     <a href="https://www.revitapidocs.com/2017.1/7f368167-6543-9be9-67a3-c6e1696ae060.htm">Document.IsWorkshared</a> is true
        ///     <a href="https://www.revitapidocs.com/2017.1/6d42ee05-5738-8685-2165-57f9809f3161.htm">Document.GetWorksharingCentralModelPath()</a>.
        /// </summary>
        [MaxLength(1024)]
        [JsonPropertyName(PropertyNames.RevitDocumentModelPath)]
        public string? RevitDocumentModelPath { get; init; }

        /// <summary>
        ///     Dynamic properties data.
        /// </summary>
        [MaxLength(8000)]
        [JsonPropertyName(PropertyNames.LogEvent)]
        [JsonConverter(typeof(DynamicDataJsonConverter))]
        public string? LogEvent { get; init; }
    }
}