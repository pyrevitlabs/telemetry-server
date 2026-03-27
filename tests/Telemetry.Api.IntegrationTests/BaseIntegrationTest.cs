using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Telemetry.Api.Application.DTOs;
using TUnit.Core.Interfaces;

namespace Telemetry.Api.IntegrationTests
{
    public abstract class BaseIntegrationTest : WebApplicationFactory<Program>, IAsyncInitializer, IAsyncDisposable
    {
        protected HttpClient Client { get; private set; } = null!;

        protected abstract string DbProvider { get; }
        protected abstract string ConnectionString { get; }
        protected virtual string? MongoDbDatabaseName => null;

        public new virtual async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
        }

        public virtual async Task InitializeAsync()
        {
            Client = CreateClient();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSetting("DbProvider", DbProvider);
            builder.UseSetting("ConnectionStrings:DefaultConnection", ConnectionString);

            if (MongoDbDatabaseName != null)
            {
                builder.UseSetting("MongoDbDatabaseName", MongoDbDatabaseName);
            }

            builder.ConfigureTestServices(services =>
            {
                // config from Program.cs
            });
        }

        protected ScriptRecordDto CreateSampleScriptDto()
        {
            return new ScriptRecordDto
            {
                SessionId = Guid.NewGuid(),
                Meta = new MetaDto {SchemaVersion = new Version(2, 0)},
                Timestamp = DateTimeOffset.UtcNow,
                Username = "testuser",
                HostUsername = "hostuser",
                RevitBuild = "2024.1",
                RevitVersion = "2024",
                PyRevitVersion = "4.8.14",
                CloneName = "master",
                IsDebug = false,
                IsConfig = false,
                IsExecFromGui = true,
                ExecId = Guid.NewGuid().ToString(),
                ExecTimestamp = DateTimeOffset.UtcNow,
                CommandBundle = "test.extension",
                CommandExtension = "test",
                CommandName = "Test Command",
                CommandUniqueName = "test.command.unique",
                DocumentName = "test.rvt",
                DocumentPath = @"C:\test.rvt",
                ResultCode = 0,
                ScriptPath = @"C:\scripts\test.py",
                Trace = new TraceInfoDto
                {
                    Message = "test trace message",
                    Engine = new EngineInfoDto
                    {
                        Type = "ironpython",
                        Version = "2.7.12",
                        SysPath = ["path1", "path2"],
                        Configs = "some configs"
                    }
                },
                CommandResults = "success"
            };
        }

        protected EventRecordDto CreateSampleEventDto()
        {
            return new EventRecordDto
            {
                HandlerId = Guid.NewGuid(),
                Meta = new MetaDto {SchemaVersion = new Version(2, 0)},
                EventType = "app-startup",
                Status = "success",
                Timestamp = DateTimeOffset.UtcNow,
                Username = "testuser",
                HostUsername = "hostuser",
                RevitBuild = "2024.1",
                RevitVersion = "2024",
                Cancelled = false,
                DocumentId = 12345,
                DocumentType = "Project",
                DocumentTemplate = "template.rte",
                DocumentName = "test.rvt",
                DocumentPath = "C:\\test.rvt",
                ProjectName = "Test Project",
                ProjectNum = "123-456",
                EventArgs = "some args"
            };
        }
    }
}