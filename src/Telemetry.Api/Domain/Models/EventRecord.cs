using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Telemetry.Api.Domain.Constants;

namespace Telemetry.Api.Domain.Models
{
    /// <summary>
    ///     Event record information.
    /// </summary>
    [Table("events")]
    public class EventRecord
    {
        /// <summary>
        /// Event id.
        /// </summary>
        [Key]
        [Column(PropertyNames.Id)]
        public Guid Id { get; init; }
     
        /// <summary>
        ///     Unique event id.
        /// </summary>
        [Column(PropertyNames.HandlerId)]
        public Guid HandlerId { get; init; }

        /// <summary>
        ///     Event type name.
        /// </summary>
        [MaxLength(100)]
        [Column(PropertyNames.EventType)]
        public required string EventType { get; init; }

        /// <summary>
        ///     Revit event
        ///     <a href="https://www.revitapidocs.com/2022/a739b1f8-6b3b-a95b-b536-6e5d00d12e4e.htm">status</a>.
        /// </summary>
        [MaxLength(100)]
        [Column(PropertyNames.Status)]
        public string? Status { get; init; }

        /// <summary>
        ///     When event started.
        /// </summary>
        [Column(PropertyNames.Timestamp)]
        public DateTimeOffset Timestamp { get; init; }

        /// <summary>
        ///     <a href="https://www.revitapidocs.com/2022/2a7c8664-de0d-7a43-e670-2e733e579609.htm">Username</a>
        ///     who use Autodesk Revit (sets in options).
        /// </summary>
        [MaxLength(100)]
        [Column(PropertyNames.Username)]
        public required string Username { get; init; }

        /// <summary>
        ///     <a href="https://learn.microsoft.com/en-us/dotnet/api/system.environment.username">Username</a>
        ///     who logged in Windows.
        /// </summary>
        [MaxLength(100)]
        [Column(PropertyNames.HostUsername)]
        public string? HostUsername { get; init; }

        /// <summary>
        ///     Internal
        ///     <a href="https://www.revitapidocs.com/2022/c5963cab-c85b-561b-1ea2-b9d11b58050c.htm">build number</a>
        ///     of the Autodesk Revit application.
        /// </summary>
        [MaxLength(100)]
        [Column(PropertyNames.RevitBuild)]
        public required string RevitBuild { get; init; }

        /// <summary>
        ///     Return the
        ///     <a href="https://www.revitapidocs.com/2022/35b18b73-4c47-fee3-d2f9-21298f029f7f.htm">primary version</a>
        ///     of the Revit application.
        /// </summary>
        [MaxLength(100)]
        [Column(PropertyNames.RevitVersion)]
        public required string RevitVersion { get; init; }

        /// <summary>
        ///     If event was cancelled <see langword="true" />, otherwise  <see langword="false" />.
        ///     <br />RevitAPIEventArgs
        ///     <a href="https://www.revitapidocs.com/2022/5627aeaa-9d9c-dcbe-b34f-db40f1c025be.htm">IsCancelled</a> method.
        /// </summary>
        [Column(PropertyNames.Cancelled)]
        public bool? Cancelled { get; init; }
        
        /// <summary>
        ///     If event can cancel <see langword="true" />, otherwise  <see langword="false" />.
        ///     <br />RevitAPIEventArgs
        ///     <a href="https://www.revitapidocs.com/2022/a393138a-34b5-1724-aa69-92cef651482b.htm">Cancellable</a> property.
        /// </summary>
        [Column(PropertyNames.Cancellable)]
        public bool? Cancellable { get; init; }
        
        /// <summary>
        ///     Id of the document that has just been closed.
        ///     <br />DocumentClosingEventArgs
        ///     <a href="https://www.revitapidocs.com/2022/c1f2fa0f-0071-caef-34d7-b966458fc60b.htm">DocumentId</a> property.
        ///     <br />DocumentClosedEventArgs
        ///     <a href="https://www.revitapidocs.com/2022/b9c0620b-817c-c85e-322e-c56cc942eda2.htm">DocumentId</a> property.
        /// </summary>
        [Column(PropertyNames.DocumentId)]
        public int DocumentId { get; init; }
        
        /// <summary>
        ///     Type of the document, e.g. Project or Template.
        ///     <br /><a href="https://www.revitapidocs.com/2022/f980a45e-61f3-de6e-9af3-9277d5537b4b.htm">DocumentType</a>
        ///     enumeration.
        ///     <br />DocumentOpeningEventArgs
        ///     <a href="https://www.revitapidocs.com/2022/0d94b412-5685-dd91-7cae-e987e0e2ebbb.htm">DocumentType</a> property.
        ///     <br />DocumentCreatingEventArgs
        ///     <a href="https://www.revitapidocs.com/2022/a7986733-b89f-3cb3-0e60-96c2a1beb1f5.htm">DocumentType</a> property.
        /// </summary>
        [MaxLength(100)]
        [Column(PropertyNames.DocumentType)]
        public string? DocumentType { get; init; }

        /// <summary>
        ///     Document template name.
        ///     <br />DocumentCreatingEventArgs
        ///     <a href="https://www.revitapidocs.com/2022/634fd76a-8466-b705-b20d-b5a0c7303a80.htm">Template</a> property.
        /// </summary>
        [MaxLength(100)] 
        [Column(PropertyNames.DocumentTemplate)]
        public string? DocumentTemplate { get; init; }

        /// <summary>
        ///     Document <a href="https://www.revitapidocs.com/2022/4cee7916-d799-fc83-daf3-97cb03900b72.htm">Title</a> property.
        /// </summary>
        [MaxLength(250)]
        [Column(PropertyNames.DocumentName)]
        public string? DocumentName { get; init; }

        /// <summary>
        ///     Document <a href="https://www.revitapidocs.com/2022/8a92a6fd-ce25-cd86-2068-f9dcb24d72d6.htm">PathName</a>
        ///     property.
        /// </summary>
        [MaxLength(1024)]
        [Column(PropertyNames.DocumentPath)]
        public string? DocumentPath { get; init; }

        /// <summary>
        ///     Project name
        ///     <a href="https://www.revitapidocs.com/2022/fb011c91-be7e-f737-28c7-3f1e1917a0e0.htm">(BuiltInParameter.PROJECT_NAME)</a>.
        /// </summary>
        [MaxLength(250)]
        [Column(PropertyNames.ProjectName)]
        public string? ProjectName { get; init; }

        /// <summary>
        ///     Project number
        ///     <a href="https://www.revitapidocs.com/2022/fb011c91-be7e-f737-28c7-3f1e1917a0e0.htm">(BuiltInParameter.PROJECT_NUMBER)</a>.
        /// </summary>
        [MaxLength(100)]
        [Column(PropertyNames.ProjectNum)]
        public string? ProjectNum { get; init; }

        /// <summary>
        ///     Dynamic event args data.
        /// </summary>
        [MaxLength(8000)]
        [Column(PropertyNames.EventArgs)]
        public string? EventArgs { get; init; }
    }
}