using System.Text;
using Telemetry.Api.Application.DTOs;
using Testcontainers.Oracle;

namespace Telemetry.Api.IntegrationTests
{
    [InheritsTests]
    public class OracleIntegrationTests : BaseIntegrationTest
    {
        private readonly OracleContainer _container = new OracleBuilder("gvenzl/oracle-free:23-slim")
            .Build();

        protected override string DbProvider => "oracle";
        protected override string ConnectionString => _container.GetConnectionString().Replace("XEPDB1", "FREEPDB1");

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
            await Assert.That(content).Contains("Oracle.EntityFrameworkCore");
        }
    }
}