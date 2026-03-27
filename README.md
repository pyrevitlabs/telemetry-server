# Telemetry Server for pyRevit

A high-performance .NET 10 Web API service designed to collect and store telemetry data
from [pyRevit](https://github.com/pyrevitlabs/pyRevit). It supports multiple database providers and can be easily
deployed using Docker.

## Project Structure

The project follows a Clean Architecture approach:

- **src/Telemetry.Api**: Main application source code.
    - **Application**: Business logic, DTOs, mappings, and service interfaces.
    - **Domain**: Core domain models and entities.
    - **Infrastructure**: Database persistence, Entity Framework Core contexts, and external service implementations.
    - **Web**: ASP.NET Core controllers, middleware, and API configuration.
- **src/Telemetry.Migrations.***: Separate projects for Entity Framework Core migrations (PostgreSQL, SQL Server, SQLite, Oracle).
- **tests**: Comprehensive test suite using [TUnit](https://github.com/thomhurst/TUnit).
    - **Telemetry.Api.UnitTests**: Isolated tests for business logic and mappings.
    - **Telemetry.Api.IntegrationTests**: End-to-end tests for API endpoints and database interactions.
- **telemetry-db**: Local directory for database persistence (used by Docker).

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/get-started) and [Docker Compose](https://docs.docker.com/compose/install/)

## Compilation

To build the project locally, run the following command from the root directory:

```bash
dotnet build telemetry-server.slnx
```

To run the application:

```bash
dotnet run --project src/Telemetry.Api/Telemetry.Api.csproj
```

## Docker Deployment

The project provides several Docker Compose configurations to support different database backends.

### General Usage

To start the API using the pre-built image, you can pull the latest version:

```bash
docker pull pyrevit-telemetry:latest
```

To start the API with a specific database, use the following command structure:

```bash
docker compose -f docker-compose.yml -f docker-compose.<db-type>.yml up -d
```

### Supported Database Variants

| Database       | Command                                                                     |
|:---------------|:----------------------------------------------------------------------------|
| **PostgreSQL** | `docker compose -f docker-compose.yml -f docker-compose.postgres.yml up -d` |
| **SQL Server** | `docker compose -f docker-compose.yml -f docker-compose.mssql.yml up -d`    |
| **SQLite**     | `docker compose -f docker-compose.yml -f docker-compose.sqlite.yml up -d`   |
| **Oracle**     | `docker compose -f docker-compose.yml -f docker-compose.oracle.yml up -d`   |
| **MongoDB**    | `docker compose -f docker-compose.yml -f docker-compose.mongodb.yml up -d`  |

> ❗IMPORTANT
> - **MySQL**: Support is currently unavailable because the necessary Entity Framework Core libraries have not yet been ported to .NET 10.
> - **SQL Server & Oracle**: While these providers are implemented, they have not been fully tested in a .NET 10 environment. Full verification and ongoing support for these databases are left to the community.

### Environment Variables

Each database variant requires specific environment variables (usually defined in a `.env` file or passed directly). Key
variables include:

- `DbProvider`: Specifies the database provider (e.g., `postgres`, `mssql`, `sqlite`, `oracle`, `mongodb`).
- `ConnectionStrings__DefaultConnection`: The connection string for the selected database.
- `ASPNETCORE_HTTP_PORTS`: The port the API listens to on (default: `8080`).

## API Endpoints

The API is versioned (currently `v2`).

- `POST /api/v2/scripts`: Submit pyRevit script execution telemetry.
- `POST /api/v2/events`: Submit pyRevit event telemetry.
- `GET /api/v2/status`: Check service and database connection status.
- `GET /metrics`: Prometheus metrics for monitoring.
- `GET /swagger`: OpenAPI/Swagger documentation (in Development mode).

## API Documentation

When running in **Development** mode, the application provides interactive API documentation using [Swagger (OpenAPI)](https://swagger.io/).

To access the Swagger UI:
1. Start the application in Development mode (`ASPNETCORE_ENVIRONMENT=Development`).
2. Navigate to `http://localhost:8080/swagger` (or your configured port).

From the Swagger UI, you can explore all available endpoints, view request/response schemas, and test the API directly from your browser.

## Monitoring

The application exposes Prometheus metrics at the `/metrics` endpoint. This can be used to monitor the service's health, request rates, and performance using [Prometheus](https://prometheus.io/) and [Grafana](https://grafana.com/).

To view the metrics:
1. Start the application.
2. Navigate to `http://localhost:8080/metrics` (or your configured port).

## Testing

The project uses [TUnit](https://github.com/thomhurst/TUnit), a modern testing framework for .NET, taking advantage of
the latest features in .NET 10.

### Unit Tests

Unit tests focus on isolated business logic and data mappings. They do not require any external dependencies.

To run unit tests:

```bash
dotnet run --project tests/Telemetry.Api.UnitTests/Telemetry.Api.UnitTests.csproj
```

### Integration Tests

Integration tests verify the end-to-end functionality of the API, including database persistence. These tests
use [Testcontainers](https://testcontainers.com/) to spin up actual database instances (except for SQLite) during the
test execution.

**Prerequisites:** Docker must be running on your machine to execute integration tests for PostgreSQL, SQL Server,
Oracle, and MongoDB.

To run integration tests:

```bash
dotnet run --project tests/Telemetry.Api.IntegrationTests/Telemetry.Api.IntegrationTests.csproj
```

## Manual Deployment (Without Docker)

To run the server manually, follow these steps to build, configure, and publish the application.

### Build and Publish

1.  **Publish the project:**
    Run the following command to create a self-contained or framework-dependent deployment:

    ```bash
    dotnet publish src/Telemetry.Api/Telemetry.Api.csproj -c Release -o ./publish
    ```

2.  **Navigate to the publish directory:**

    ```bash
    cd ./publish
    ```

### Configuration (appsettings.json)

Before running the server, you must configure the database provider and connection string in `appsettings.json`.

1.  **Open `appsettings.json`** and add the following configuration:

    ```json
    {
      "DbProvider": "postgres",
      "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Database=telemetry;Username=postgres;Password=password"
      },
      "MongoDbDatabaseName": "telemetry",
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft.AspNetCore": "Warning"
        }
      },
      "AllowedHosts": "*"
    }
    ```

2.  **Configuration Options:**
    - `DbProvider`: Set to one of `postgres`, `mssql`, `sqlite`, `oracle`, `mongodb`.
    - `ConnectionStrings:DefaultConnection`: Provide the appropriate connection string for your database.
    - `MongoDbDatabaseName`: (Optional) Required only if using `mongodb`.

### Running the Server

Start the application using the `dotnet` CLI:

```bash
dotnet Telemetry.Api.dll
```

The server will start and listen on the default ports (usually `http://localhost:5000` or as configured via environment variables).

## Database Migrations

The project uses Entity Framework Core (EF Core) to manage relational database schemas (PostgreSQL, SQL Server, SQLite, Oracle).

### Prerequisites

You need the `dotnet-ef` tool installed globally:

```bash
dotnet tool install --global dotnet-ef
```

### Creating a New Migration

If you modify the data models in the `Domain` or `Infrastructure` layers, you need to generate a new migration for each supported database provider. Each database has its own project for migrations.

To generate migrations correctly, set the required environment variables for the target database and run the `dotnet ef migrations add` command from the root directory.

#### Oracle
```bash
DbProvider=oracle ConnectionStrings__DefaultConnection="Data Source=localhost:1521/xe;User Id=test;Password=test" \
dotnet ef migrations add <MigrationName> \
    --project src/Telemetry.Migrations.Oracle \
    --startup-project src/Telemetry.Api \
    --context ApplicationDbContext
```

#### PostgreSQL
```bash
DbProvider=postgres ConnectionStrings__DefaultConnection="Host=localhost;Database=test;Username=test;Password=test" \
dotnet ef migrations add <MigrationName> \
    --project src/Telemetry.Migrations.Postgres \
    --startup-project src/Telemetry.Api \
    --context ApplicationDbContext
```

#### SQLite
```bash
DbProvider=sqlite ConnectionStrings__DefaultConnection="Data Source=/app/data/telemetry.db" \
dotnet ef migrations add <MigrationName> \
    --project src/Telemetry.Migrations.Sqlite \
    --startup-project src/Telemetry.Api \
    --context ApplicationDbContext
```

#### SQL Server
```bash
DbProvider=mssql ConnectionStrings__DefaultConnection="Server=localhost;Database=test;User Id=sa;Password=test" \
dotnet ef migrations add <MigrationName> \
    --project src/Telemetry.Migrations.SqlServer \
    --startup-project src/Telemetry.Api \
    --context ApplicationDbContext
```

### Reviewing the generated files

New migration files will be created in their respective projects under the `Migrations` directory (e.g., `src/Telemetry.Migrations.Postgres/Migrations`).

### Applying Migrations

-   **Automatic:** The server automatically applies pending migrations on startup for all relational database providers.
-   **Manual:** You can manually update the database using the following command (example for SQLite):

    ```bash
    DbProvider=sqlite ConnectionStrings__DefaultConnection="Data Source=/app/data/telemetry.db" \
    dotnet ef database update \
        --project src/Telemetry.Migrations.Sqlite \
        --startup-project src/Telemetry.Api \
        --context ApplicationDbContext
    ```

> ❗NOTE  
> **MongoDB** does not use EF Core migrations. Changes to the MongoDB data structure are handled by the application at runtime (schema-less or through driver-level configuration).

## License

This project is licensed under the terms of the license included in the root directory.
