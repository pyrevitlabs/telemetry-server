using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Telemetry.Api.Domain.Models
{
    /// <summary>
    ///     Script record information.
    /// </summary>
    [Table("scripts")]
    public class ScriptRecord
    {
        /// <summary>
        /// Scripts id.
        /// </summary>
        public Guid Id { get; init; }
        
        /// <summary>
        ///     Unique session id (created when revit is opened).
        /// </summary>
        [BsonElement("sessionid")]
        public Guid SessionId { get; init; }
        
        /// <summary>
        ///     Information about telemetry record.
        /// </summary>
        [BsonElement("meta")]
        public required MetaRecord Meta { get; init; }
        
        /// <summary>
        ///     When script started.
        /// </summary>
        [BsonElement("timestamp")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTimeOffset Timestamp { get; init; }

        /// <summary>
        ///     <a href="https://www.revitapidocs.com/2022/2a7c8664-de0d-7a43-e670-2e733e579609.htm">Username</a>
        ///     who use Autodesk Revit (sets in options).
        /// </summary>
        [MaxLength(100)]
        [BsonElement("username")]
        public required string Username { get; init; }

        /// <summary>
        ///     <a href="https://learn.microsoft.com/en-us/dotnet/api/system.environment.username">Username</a>
        ///     who logged in Windows.
        /// </summary>
        [MaxLength(100)]
        [BsonElement("host_user")]
        public string? HostUsername { get; init; }

        /// <summary>
        ///     Internal
        ///     <a href="https://www.revitapidocs.com/2022/c5963cab-c85b-561b-1ea2-b9d11b58050c.htm">build number</a>
        ///     of the Autodesk Revit application.
        /// </summary>
        [MaxLength(100)] 
        [BsonElement("revitbuild")]
        public required string RevitBuild { get; init; }

        /// <summary>
        ///     Return the
        ///     <a href="https://www.revitapidocs.com/2022/35b18b73-4c47-fee3-d2f9-21298f029f7f.htm">primary version</a>
        ///     of the Revit application.
        /// </summary>
        [MaxLength(100)] 
        [BsonElement("revit")]
        public required string RevitVersion { get; init; }

        /// <summary>
        ///     pyrevit build version.
        /// </summary>
        [MaxLength(100)] 
        [BsonElement("pyrevit")]
        public required string PyRevitVersion { get; init; }

        /// <summary>
        ///     pyrevit
        ///     <a href="https://pyrevitlabs.notion.site/Manage-pyRevit-clones-e9f789f9431346b482021f2a87a6dabf">clone name</a>.
        /// </summary>
        [MaxLength(100)] 
        [BsonElement("clone")]
        public required string CloneName { get; init; }

        /// <summary>
        ///     pyrevit
        ///     <a
        ///         href="https://pyrevitlabs.notion.site/Button-Click-Modes-c829c5a60ddb4c3e819bc93dfbc3c98b#a09dd3a1d0634d4eab964eec07d74286">
        ///         debug
        ///         mode
        ///     </a>.
        /// </summary>
        [BsonElement("debug")]
        public bool IsDebug { get; init; }
        
        /// <summary>
        ///     pyrevit
        ///     <a href="https://pyrevitlabs.notion.site/Button-Click-Modes-c829c5a60ddb4c3e819bc93dfbc3c98b">config mode</a>.
        /// </summary>
        [BsonElement("config")]
        public bool IsConfig { get; init; }
        
        /// <summary>
        ///     If script was run from GUI (Click Revit Ribbon) <see langword="true" />, otherwise  <see langword="false" />.
        /// </summary>
        [BsonElement("from_gui")]
        public bool IsExecFromGui { get; init; }

        /// <summary>
        ///     Unique execution id.
        /// </summary>
        [MaxLength(100)]
        [BsonElement("exec_id")]
        public required string ExecId { get; init; }

        /// <summary>
        ///     When script executed.
        /// </summary>
        [BsonElement("exec_timestamp")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTimeOffset ExecTimestamp { get; init; }

        /// <summary>
        ///     Command bundle name.
        /// </summary>
        [MaxLength(250)] 
        [BsonElement("commandbundle")]
        public required string CommandBundle { get; init; }

        /// <summary>
        ///     Command extension name.
        /// </summary>
        [MaxLength(250)] 
        [BsonElement("commandextension")]
        public required string CommandExtension { get; init; }

        /// <summary>
        ///     Command name.
        /// </summary>
        [MaxLength(250)] 
        [BsonElement("commandname")]
        public required string CommandName { get; init; }

        /// <summary>
        ///     Command unique name.
        /// </summary>
        [MaxLength(500)]
        [BsonElement("commanduniquename")]
        public required string CommandUniqueName { get; init; }

        /// <summary>
        ///     Document <a href="https://www.revitapidocs.com/2022/4cee7916-d799-fc83-daf3-97cb03900b72.htm">Title</a> property.
        /// </summary>
        [MaxLength(250)] 
        [BsonElement("docname")]
        public string? DocumentName { get; init; }

        /// <summary>
        ///     Document <a href="https://www.revitapidocs.com/2022/8a92a6fd-ce25-cd86-2068-f9dcb24d72d6.htm">PathName</a>
        ///     property.
        /// </summary>
        [MaxLength(1024)] 
        [BsonElement("docpath")]
        public string? DocumentPath { get; init; }

        /// <summary>
        ///     Script executed result code.
        ///     <br /><a href="https://www.revitapidocs.com/2022/e6cebb3c-0c3f-7dc4-2063-e5df0a00b2f5.htm">ResultCode</a>
        ///     enumeration.
        /// </summary>
        [BsonElement("resultcode")]
        public int ResultCode { get; init; }

        /// <summary>
        ///     Executed script path.
        /// </summary>
        [MaxLength(1024)]
        [BsonElement("scriptpath")]
        public required string ScriptPath { get; init; }

        /// <summary>
        ///     Information about execution.
        /// </summary>
        [BsonElement("trace")]
        public required TraceInfo Trace { get; init; }

        /// <summary>
        ///     Additional command results.
        /// </summary>
        [MaxLength(8000)] 
        [BsonElement("commandresults")]
        public string? CommandResults { get; init; }
    }
}