using Telemetry.Api.Application.DTOs;
using Telemetry.Api.Application.Mappings;
using Telemetry.Api.Domain.Models;

namespace Telemetry.Api.UnitTests.Application.Mappings
{
    public class TelemetryMappingExtensionsTests
    {
        [Test]
        public async Task MetaDto_ToModel_ShouldMapCorrectly()
        {
            // Arrange
            MetaDto dto = new() {SchemaVersion = new Version(2, 1)};

            // Act
            MetaRecord model = dto.ToModel();

            // Assert
            await Assert.That(model.SchemaVersion).IsEqualTo(dto.SchemaVersion);
        }

        [Test]
        public async Task MetaRecord_ToDto_ShouldMapCorrectly()
        {
            // Arrange
            MetaRecord model = new() {SchemaVersion = new Version(2, 1)};

            // Act
            MetaDto dto = model.ToDto();

            // Assert
            await Assert.That(dto.SchemaVersion).IsEqualTo(model.SchemaVersion);
        }

        [Test]
        public async Task EngineInfoDto_ToModel_ShouldMapCorrectly()
        {
            // Arrange
            EngineInfoDto dto = new()
            {
                Type = "CPython", Version = "3.8", SysPath = new[] {"path1", "path2"}, Configs = "some config"
            };

            // Act
            EngineInfo model = dto.ToModel();

            // Assert
            await Assert.That(model.Type).IsEqualTo(dto.Type);
            await Assert.That(model.Version).IsEqualTo(dto.Version);
            await Assert.That(model.SysPaths).IsEquivalentTo(dto.SysPath);
            await Assert.That(model.Configs).IsEqualTo(dto.Configs);
        }

        [Test]
        public async Task TraceInfoDto_ToModel_ShouldMapCorrectly()
        {
            // Arrange
            TraceInfoDto dto = new()
            {
                Message = "Test message", Engine = new EngineInfoDto {Type = "IronPython", Version = "2.7"}
            };

            // Act
            TraceInfo model = dto.ToModel();

            // Assert
            await Assert.That(model.Message).IsEqualTo(dto.Message);
            await Assert.That(model.Engine.Type).IsEqualTo(dto.Engine.Type);
            await Assert.That(model.Engine.Version).IsEqualTo(dto.Engine.Version);
        }

        [Test]
        public async Task ScriptRecordDto_ToModel_ShouldMapCorrectly()
        {
            // Arrange
            ScriptRecordDto dto = new()
            {
                SessionId = Guid.NewGuid(),
                Meta = new MetaDto {SchemaVersion = new Version(2, 0)},
                Timestamp = DateTimeOffset.Now,
                Username = "user",
                HostUsername = "host",
                RevitBuild = "2024.1",
                RevitVersion = "2024",
                PyRevitVersion = "4.8.12",
                CloneName = "master",
                IsDebug = true,
                IsConfig = true,
                IsExecFromGui = true,
                ExecId = "exec-123",
                ExecTimestamp = DateTimeOffset.Now.AddSeconds(-10),
                CommandBundle = "bundle",
                CommandExtension = "ext",
                CommandName = "cmd",
                CommandUniqueName = "unique.cmd",
                DocumentName = "doc",
                DocumentPath = "C:/path/to/doc.rvt",
                ResultCode = 0,
                ScriptPath = "C:/path/to/script.py",
                Trace = new TraceInfoDto
                {
                    Message = "trace", Engine = new EngineInfoDto {Type = "python", Version = "3"}
                },
                CommandResults = "results"
            };

            // Act
            ScriptRecord model = dto.ToModel();

            // Assert
            await Assert.That(model.Id).IsNotEqualTo(Guid.Empty);
            await Assert.That(model.SessionId).IsEqualTo(dto.SessionId);
            await Assert.That(model.Timestamp).IsEqualTo(dto.Timestamp);
            await Assert.That(model.Username).IsEqualTo(dto.Username);
            await Assert.That(model.HostUsername).IsEqualTo(dto.HostUsername);
            await Assert.That(model.RevitBuild).IsEqualTo(dto.RevitBuild);
            await Assert.That(model.RevitVersion).IsEqualTo(dto.RevitVersion);
            await Assert.That(model.PyRevitVersion).IsEqualTo(dto.PyRevitVersion);
            await Assert.That(model.CloneName).IsEqualTo(dto.CloneName);
            await Assert.That(model.IsDebug).IsEqualTo(dto.IsDebug);
            await Assert.That(model.IsConfig).IsEqualTo(dto.IsConfig);
            await Assert.That(model.IsExecFromGui).IsEqualTo(dto.IsExecFromGui);
            await Assert.That(model.ExecId).IsEqualTo(dto.ExecId);
            await Assert.That(model.ExecTimestamp).IsEqualTo(dto.ExecTimestamp);
            await Assert.That(model.CommandBundle).IsEqualTo(dto.CommandBundle);
            await Assert.That(model.CommandExtension).IsEqualTo(dto.CommandExtension);
            await Assert.That(model.CommandName).IsEqualTo(dto.CommandName);
            await Assert.That(model.CommandUniqueName).IsEqualTo(dto.CommandUniqueName);
            await Assert.That(model.DocumentName).IsEqualTo(dto.DocumentName);
            await Assert.That(model.DocumentPath).IsEqualTo(dto.DocumentPath);
            await Assert.That(model.ResultCode).IsEqualTo(dto.ResultCode);
            await Assert.That(model.ScriptPath).IsEqualTo(dto.ScriptPath);
            await Assert.That(model.Trace.Message).IsEqualTo(dto.Trace.Message);
            await Assert.That(model.CommandResults).IsEqualTo(dto.CommandResults);
        }

        [Test]
        public async Task ScriptRecord_ToDto_ShouldMapCorrectly()
        {
            // Arrange
            ScriptRecord model = new()
            {
                Id = Guid.NewGuid(),
                SessionId = Guid.NewGuid(),
                Timestamp = DateTimeOffset.Now,
                Username = "user",
                HostUsername = "host",
                RevitBuild = "2024.1",
                RevitVersion = "2024",
                PyRevitVersion = "4.8.12",
                CloneName = "master",
                IsDebug = true,
                IsConfig = true,
                IsExecFromGui = true,
                ExecId = "exec-123",
                ExecTimestamp = DateTimeOffset.Now.AddSeconds(-10),
                CommandBundle = "bundle",
                CommandExtension = "ext",
                CommandName = "cmd",
                CommandUniqueName = "unique.cmd",
                DocumentName = "doc",
                DocumentPath = "C:/path/to/doc.rvt",
                ResultCode = 0,
                ScriptPath = "C:/path/to/script.py",
                Trace = new TraceInfo
                {
                    Message = "trace", Engine = new EngineInfo {Type = "python", Version = "3"}
                },
                CommandResults = "results"
            };

            // Act
            ScriptRecordDto dto = model.ToDto();

            // Assert
            await Assert.That(dto.SessionId).IsEqualTo(model.SessionId);
            await Assert.That(dto.Timestamp).IsEqualTo(model.Timestamp);
            await Assert.That(dto.Username).IsEqualTo(model.Username);
            await Assert.That(dto.HostUsername).IsEqualTo(model.HostUsername);
            await Assert.That(dto.RevitBuild).IsEqualTo(model.RevitBuild);
            await Assert.That(dto.RevitVersion).IsEqualTo(model.RevitVersion);
            await Assert.That(dto.PyRevitVersion).IsEqualTo(model.PyRevitVersion);
            await Assert.That(dto.CloneName).IsEqualTo(model.CloneName);
            await Assert.That(dto.IsDebug).IsEqualTo(model.IsDebug);
            await Assert.That(dto.IsConfig).IsEqualTo(model.IsConfig);
            await Assert.That(dto.IsExecFromGui).IsEqualTo(model.IsExecFromGui);
            await Assert.That(dto.ExecId).IsEqualTo(model.ExecId);
            await Assert.That(dto.ExecTimestamp).IsEqualTo(model.ExecTimestamp);
            await Assert.That(dto.CommandBundle).IsEqualTo(model.CommandBundle);
            await Assert.That(dto.CommandExtension).IsEqualTo(model.CommandExtension);
            await Assert.That(dto.CommandName).IsEqualTo(model.CommandName);
            await Assert.That(dto.CommandUniqueName).IsEqualTo(model.CommandUniqueName);
            await Assert.That(dto.DocumentName).IsEqualTo(model.DocumentName);
            await Assert.That(dto.DocumentPath).IsEqualTo(model.DocumentPath);
            await Assert.That(dto.ResultCode).IsEqualTo(model.ResultCode);
            await Assert.That(dto.ScriptPath).IsEqualTo(model.ScriptPath);
            await Assert.That(dto.Trace.Message).IsEqualTo(model.Trace.Message);
            await Assert.That(dto.CommandResults).IsEqualTo(model.CommandResults);
        }

        [Test]
        public async Task EventRecordDto_ToModel_ShouldMapCorrectly()
        {
            // Arrange
            EventRecordDto dto = new()
            {
                HandlerId = Guid.NewGuid(),
                Meta = new MetaDto {SchemaVersion = new Version(2, 0)},
                EventType = "DocOpened",
                Status = "Success",
                Timestamp = DateTimeOffset.Now,
                Username = "user",
                HostUsername = "host",
                RevitBuild = "2024.1",
                RevitVersion = "2024",
                Cancelled = false,
                DocumentId = 1,
                DocumentType = "Project",
                DocumentTemplate = "template",
                DocumentName = "doc",
                DocumentPath = "C:/path/to/doc.rvt",
                ProjectName = "project",
                ProjectNum = "123",
                EventArgs = "args"
            };

            // Act
            EventRecord model = dto.ToModel();

            // Assert
            await Assert.That(model.Id).IsNotEqualTo(Guid.Empty);
            await Assert.That(model.HandlerId).IsEqualTo(dto.HandlerId);
            await Assert.That(model.EventType).IsEqualTo(dto.EventType);
            await Assert.That(model.Status).IsEqualTo(dto.Status);
            await Assert.That(model.Timestamp).IsEqualTo(dto.Timestamp);
            await Assert.That(model.Username).IsEqualTo(dto.Username);
            await Assert.That(model.HostUsername).IsEqualTo(dto.HostUsername);
            await Assert.That(model.RevitBuild).IsEqualTo(dto.RevitBuild);
            await Assert.That(model.RevitVersion).IsEqualTo(dto.RevitVersion);
            await Assert.That(model.Cancelled).IsEqualTo(dto.Cancelled);
            await Assert.That(model.DocumentId).IsEqualTo(dto.DocumentId);
            await Assert.That(model.DocumentType).IsEqualTo(dto.DocumentType);
            await Assert.That(model.DocumentTemplate).IsEqualTo(dto.DocumentTemplate);
            await Assert.That(model.DocumentName).IsEqualTo(dto.DocumentName);
            await Assert.That(model.DocumentPath).IsEqualTo(dto.DocumentPath);
            await Assert.That(model.ProjectName).IsEqualTo(dto.ProjectName);
            await Assert.That(model.ProjectNum).IsEqualTo(dto.ProjectNum);
            await Assert.That(model.EventArgs).IsEqualTo(dto.EventArgs);
        }

        [Test]
        public async Task EventRecord_ToDto_ShouldMapCorrectly()
        {
            // Arrange
            EventRecord model = new()
            {
                Id = Guid.NewGuid(),
                HandlerId = Guid.NewGuid(),
                EventType = "DocOpened",
                Status = "Success",
                Timestamp = DateTimeOffset.Now,
                Username = "user",
                HostUsername = "host",
                RevitBuild = "2024.1",
                RevitVersion = "2024",
                Cancelled = false,
                DocumentId = 1,
                DocumentType = "Project",
                DocumentTemplate = "template",
                DocumentName = "doc",
                DocumentPath = "C:/path/to/doc.rvt",
                ProjectName = "project",
                ProjectNum = "123",
                EventArgs = "args"
            };

            // Act
            EventRecordDto dto = model.ToDto();

            // Assert
            await Assert.That(dto.HandlerId).IsEqualTo(model.HandlerId);
            await Assert.That(dto.EventType).IsEqualTo(model.EventType);
            await Assert.That(dto.Status).IsEqualTo(model.Status);
            await Assert.That(dto.Timestamp).IsEqualTo(model.Timestamp);
            await Assert.That(dto.Username).IsEqualTo(model.Username);
            await Assert.That(dto.HostUsername).IsEqualTo(model.HostUsername);
            await Assert.That(dto.RevitBuild).IsEqualTo(model.RevitBuild);
            await Assert.That(dto.RevitVersion).IsEqualTo(model.RevitVersion);
            await Assert.That(dto.Cancelled).IsEqualTo(model.Cancelled);
            await Assert.That(dto.DocumentId).IsEqualTo(model.DocumentId);
            await Assert.That(dto.DocumentType).IsEqualTo(model.DocumentType);
            await Assert.That(dto.DocumentTemplate).IsEqualTo(model.DocumentTemplate);
            await Assert.That(dto.DocumentName).IsEqualTo(model.DocumentName);
            await Assert.That(dto.DocumentPath).IsEqualTo(model.DocumentPath);
            await Assert.That(dto.ProjectName).IsEqualTo(model.ProjectName);
            await Assert.That(dto.ProjectNum).IsEqualTo(model.ProjectNum);
            await Assert.That(dto.EventArgs).IsEqualTo(model.EventArgs);
        }
    }
}