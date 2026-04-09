using System.Text;
using Telemetry.Api.Application.DTOs;
using Testcontainers.PostgreSql;

namespace Telemetry.Api.IntegrationTests
{
    [InheritsTests]
    public class PostgresIntegrationTests : BaseIntegrationTest
    {
        private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:18-alpine")
            .Build();

        protected override string DbProvider => "postgres";
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
            await Assert.That(content).Contains("Npgsql.EntityFrameworkCore.PostgreSQL");
        }
    }
}