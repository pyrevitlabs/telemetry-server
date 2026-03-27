using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Telemetry.Api.Application.DTOs;
using Telemetry.Api.Application.Interfaces;
using Telemetry.Api.Domain.Models;
using Telemetry.Api.Web.Controllers;
using System.Text.Json;

namespace Telemetry.Api.UnitTests.Web.Controllers
{
    public class TelemetryControllerTests
    {
        private readonly TelemetryController _controller;
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly Mock<ILogger<TelemetryController>> _mockLogger;
        private readonly Mock<IServiceInfo> _mockServiceInfo;

        public TelemetryControllerTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _mockLogger = new Mock<ILogger<TelemetryController>>();
            _mockServiceInfo = new Mock<IServiceInfo>();
            _controller = new TelemetryController(_mockContext.Object, _mockLogger.Object, _mockServiceInfo.Object);

            // Mock HttpContext
            DefaultHttpContext httpContext = new();
            _controller.ControllerContext = new ControllerContext {HttpContext = httpContext};
        }

        [Test]
        public async Task PostScript_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            ScriptRecordDto dto = CreateValidScriptRecordDto();
            Mock<DbSet<ScriptRecord>> mockDbSet = new();
            _mockContext.Setup(c => c.ScriptRecords).Returns(mockDbSet.Object);

            // Act
            IActionResult result = await _controller.PostScript(dto);

            // Assert
            await Assert.That(result).IsTypeOf<OkResult>();
            _mockContext.Verify(c => c.ScriptRecords.Add(It.IsAny<ScriptRecord>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task PostEvent_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            EventRecordDto dto = CreateValidEventRecordDto();
            Mock<DbSet<EventRecord>> mockDbSet = new();
            _mockContext.Setup(c => c.EventRecords).Returns(mockDbSet.Object);

            // Act
            IActionResult result = await _controller.PostEvent(dto);

            // Assert
            await Assert.That(result).IsTypeOf<OkResult>();
            _mockContext.Verify(c => c.EventRecords.Add(It.IsAny<EventRecord>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetStatus_ShouldReturnOk_WhenDatabaseConnected()
        {
            // Arrange
            Guid serviceId = Guid.NewGuid();
            _mockContext.Setup(c => c.CanConnectAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockContext.Setup(c => c.GetDbProvider()).Returns("postgres");
            _mockContext.Setup(c => c.GetDbVersionAsync(It.IsAny<CancellationToken>())).ReturnsAsync("15.0");
            _mockServiceInfo.Setup(s => s.ServiceId).Returns(serviceId);

            // Act
            IActionResult result = await _controller.GetStatus();

            // Assert
            OkObjectResult okResult = (OkObjectResult)result;
            await Assert.That(okResult).IsTypeOf<OkObjectResult>();

            // Use reflection or JSON serialization to check anonymous type properties
            string statusJson = JsonSerializer.Serialize(okResult.Value);
            using JsonDocument doc = JsonDocument.Parse(statusJson);
            await Assert.That(doc.RootElement.GetProperty("status").GetString()).IsEqualTo("pass");
            await Assert.That(doc.RootElement.GetProperty("serviceid").GetGuid()).IsEqualTo(serviceId);
        }

        private ScriptRecordDto CreateValidScriptRecordDto()
        {
            return new ScriptRecordDto()
            {
                SessionId = Guid.NewGuid(),
                Meta = new MetaDto() {SchemaVersion = new Version(2, 0)},
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
                Trace = new TraceInfoDto()
                {
                    Message = "trace", Engine = new EngineInfoDto() {Type = "python", Version = "3"}
                },
                CommandResults = "results"
            };
        }

        private EventRecordDto CreateValidEventRecordDto()
        {
            return new EventRecordDto()
            {
                HandlerId = Guid.NewGuid(),
                Meta = new MetaDto() {SchemaVersion = new Version(2, 0)},
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
        }
    }
}