using System.Text;
using Telemetry.Api.Application.DTOs;

namespace Telemetry.Api.IntegrationTests
{
    [InheritsTests]
    public class SqliteIntegrationTests : BaseIntegrationTest
    {
        private readonly string _dbPath = $"telemetry_{Guid.NewGuid()}.db";

        protected override string DbProvider => "sqlite";
        protected override string ConnectionString => $"Data Source={_dbPath}";

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
        }

        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
            if (File.Exists(_dbPath))
            {
                File.Delete(_dbPath);
            }
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
            await Assert.That(content).Contains("Microsoft.EntityFrameworkCore.Sqlite");
        }
    }
}