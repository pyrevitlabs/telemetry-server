using Telemetry.Api.Application.DTOs;
using Telemetry.Api.Domain.Models;

namespace Telemetry.Api.Application.Mappings
{
    /// <summary>
    /// Provides extension methods for mapping telemetry-related data.
    /// </summary>
    public static class TelemetryMappingExtensions
    {
        /// <summary>
        /// Converts the specified source object to a model representation.
        /// </summary>
        /// <param name="dto">The source object to be converted.</param>
        /// <returns>A new instance of the model object created from the source.</returns>
        public static MetaRecord ToModel(this MetaDto dto)
        {
            return new MetaRecord {SchemaVersion = (Version)dto.SchemaVersion.Clone()};
        }

        /// <summary>
        /// Converts the specified source object to a data transfer object (DTO).
        /// </summary>
        /// <param name="model">The source object to be converted.</param>
        /// <returns>A new instance of the data transfer object (DTO) created from the source.</returns>
        public static MetaDto ToDto(this MetaRecord model)
        {

            return new MetaDto {SchemaVersion = (Version)model.SchemaVersion.Clone()};
        }

        /// <summary>
        /// Transforms the input data transfer object into its corresponding model representation.
        /// </summary>
        /// <param name="dto">The input data transfer object to be transformed.</param>
        /// <returns>An instance of the model created from the provided data transfer object.</returns>
        public static EngineInfo ToModel(this EngineInfoDto dto)
        {
            return new EngineInfo
            {
                Type = dto.Type, Version = dto.Version, SysPaths = dto.SysPath, Configs = dto.Configs
            };
        }

        /// <summary>
        /// Converts the specified source model into a Data Transfer Object (DTO) representation.
        /// </summary>
        /// <param name="model">The source model to be converted.</param>
        /// <returns>A new DTO instance created from the provided model.</returns>
        public static EngineInfoDto ToDto(this EngineInfo model)
        {
            return new EngineInfoDto
            {
                Type = model.Type, Version = model.Version, SysPath = model.SysPaths, Configs = model.Configs
            };
        }

        /// <summary>
        /// Transforms the provided data transfer object (DTO) into its corresponding model representation.
        /// </summary>
        /// <param name="dto">The data transfer object to be transformed.</param>
        /// <returns>A model instance derived from the given data transfer object.</returns>
        public static TraceInfo ToModel(this TraceInfoDto dto)
        {
            return new TraceInfo {Message = dto.Message, Engine = dto.Engine.ToModel()};
        }

        /// <summary>
        /// Converts the specified model object to a data transfer object (DTO) representation.
        /// </summary>
        /// <param name="model">The model object to be converted.</param>
        /// <returns>A new instance of the DTO created from the model.</returns>
        public static TraceInfoDto ToDto(this TraceInfo model)
        {
            return new TraceInfoDto {Message = model.Message, Engine = model.Engine.ToDto()};
        }

        /// <summary>
        /// Transforms the provided data transfer object into its corresponding model representation.
        /// </summary>
        /// <param name="dto">The data transfer object to be transformed.</param>
        /// <returns>The model object derived from the given data transfer object.</returns>
        public static ScriptRecord ToModel(this ScriptRecordDto dto)
        {
            return new ScriptRecord
            {
                Id = Guid.CreateVersion7(),
                SessionId = dto.SessionId,
                Timestamp = dto.Timestamp,
                Username = dto.Username,
                HostUsername = dto.HostUsername,
                RevitBuild = dto.RevitBuild,
                RevitVersion = dto.RevitVersion,
                PyRevitVersion = dto.PyRevitVersion,
                CloneName = dto.CloneName,
                IsDebug = dto.IsDebug,
                IsConfig = dto.IsConfig,
                IsExecFromGui = dto.IsExecFromGui,
                ExecId = dto.ExecId,
                ExecTimestamp = dto.ExecTimestamp,
                CommandBundle = dto.CommandBundle,
                CommandExtension = dto.CommandExtension,
                CommandName = dto.CommandName,
                CommandUniqueName = dto.CommandUniqueName,
                DocumentName = dto.DocumentName,
                DocumentPath = dto.DocumentPath,
                ResultCode = dto.ResultCode,
                ScriptPath = dto.ScriptPath,
                Trace = dto.Trace.ToModel(),
                CommandResults = dto.CommandResults
            };
        }

        /// <summary>
        /// Converts the specified source object to a data transfer object representation.
        /// </summary>
        /// <param name="model">The source object to be converted.</param>
        /// <returns>A new instance of the data transfer object created from the source.</returns>
        public static ScriptRecordDto ToDto(this ScriptRecord model)
        {
            return new ScriptRecordDto
            {
                SessionId = model.SessionId,
                Meta =  new MetaDto(){SchemaVersion = new Version(2,0,0)},
                Timestamp = model.Timestamp,
                Username = model.Username,
                HostUsername = model.HostUsername,
                RevitBuild = model.RevitBuild,
                RevitVersion = model.RevitVersion,
                PyRevitVersion = model.PyRevitVersion,
                CloneName = model.CloneName,
                IsDebug = model.IsDebug,
                IsConfig = model.IsConfig,
                IsExecFromGui = model.IsExecFromGui,
                ExecId = model.ExecId,
                ExecTimestamp = model.ExecTimestamp,
                CommandBundle = model.CommandBundle,
                CommandExtension = model.CommandExtension,
                CommandName = model.CommandName,
                CommandUniqueName = model.CommandUniqueName,
                DocumentName = model.DocumentName,
                DocumentPath = model.DocumentPath,
                ResultCode = model.ResultCode,
                ScriptPath = model.ScriptPath,
                Trace = model.Trace.ToDto(),
                CommandResults = model.CommandResults
            };
        }

        /// <summary>
        /// Transforms the provided DTO into its corresponding model representation.
        /// </summary>
        /// <param name="dto">The data transfer object to be transformed.</param>
        /// <returns>An instance of the model derived from the given DTO.</returns>
        public static EventRecord ToModel(this EventRecordDto dto)
        {
            return new EventRecord
            {
                Id = Guid.CreateVersion7(),
                HandlerId = dto.HandlerId,
                EventType = dto.EventType,
                Status = dto.Status,
                Timestamp = dto.Timestamp,
                Username = dto.Username,
                HostUsername = dto.HostUsername,
                RevitBuild = dto.RevitBuild,
                RevitVersion = dto.RevitVersion,
                Cancelled = dto.Cancelled,
                Cancellable =  dto.Cancellable,
                DocumentId = dto.DocumentId,
                DocumentType = dto.DocumentType,
                DocumentTemplate = dto.DocumentTemplate,
                DocumentName = dto.DocumentName,
                DocumentPath = dto.DocumentPath,
                ProjectName = dto.ProjectName,
                ProjectNum = dto.ProjectNum,
                EventArgs = dto.EventArgs
            };
        }

        /// <summary>
        /// Converts the specified model object to a data transfer object representation.
        /// </summary>
        /// <param name="model">The model object to be converted.</param>
        /// <returns>A new instance of the data transfer object created from the model.</returns>
        public static EventRecordDto ToDto(this EventRecord model)
        {
            return new EventRecordDto
            {
                HandlerId = model.HandlerId,
                Meta =  new MetaDto(){SchemaVersion = new Version(2,0,0)},
                EventType = model.EventType,
                Status = model.Status,
                Timestamp = model.Timestamp,
                Username = model.Username,
                HostUsername = model.HostUsername,
                RevitBuild = model.RevitBuild,
                RevitVersion = model.RevitVersion,
                Cancelled = model.Cancelled,
                Cancellable = model.Cancellable,
                DocumentId = model.DocumentId,
                DocumentType = model.DocumentType,
                DocumentTemplate = model.DocumentTemplate,
                DocumentName = model.DocumentName,
                DocumentPath = model.DocumentPath,
                ProjectName = model.ProjectName,
                ProjectNum = model.ProjectNum,
                EventArgs = model.EventArgs
            };
        }
    }
}