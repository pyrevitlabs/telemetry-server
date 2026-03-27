using Telemetry.Api.Application.DTOs;
using Testcontainers.MsSql;

namespace Telemetry.Api.IntegrationTests
{
    public class MsSqlIntegrationTests : BaseIntegrationTest
    {
        private readonly MsSqlContainer _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest")
            .Build();

        protected override string DbProvider => "mssql";
        protected override string ConnectionString => _container.GetConnectionString();

        public override async Task InitializeAsync()
        {
            await _container.StartAsync();
            await base.InitializeAsync();
        }

        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
            await _container.StopAsync();
            await _container.DisposeAsync();
        }

        [Test]
        public async Task GetStatus_ReturnsOk()
        {
            // Act
            HttpResponseMessage response = await Client.GetAsync("/api/v2/status");

            // Assert
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            await Assert.That(content).Contains("\"status\":\"pass\"");
            await Assert.That(content).Contains("Microsoft.EntityFrameworkCore.SqlServer");
        }

        [Test]
        public async Task PostScript_ReturnsOk()
        {
            // Arrange
            ScriptRecordDto dto = CreateSampleScriptDto();

            // Act
            HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v2/scripts", dto);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task PostEvent_ReturnsOk()
        {
            // Arrange
            EventRecordDto dto = CreateSampleEventDto();

            // Act
            HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v2/events", dto);

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}