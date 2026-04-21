using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Prometheus;
using Telemetry.Api.Application.Interfaces;
using Telemetry.Api.Application.Services;
using Telemetry.Api.Infrastructure.Persistence;
using Serilog;
using MongoDB.Driver;
using System.Reflection;
using System.Runtime.Loader;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger);

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.RequestMethod |
                            HttpLoggingFields.RequestPath |
                            HttpLoggingFields.ResponseStatusCode |
                            HttpLoggingFields.Duration;

    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    string? dbProvider = builder.Configuration.GetValue<string>("DbProvider")?.ToLower();

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Missing connection string");
    }

    switch (dbProvider)
    {
        case "oracle":
            Log.Information("Using Oracle database");
            options.UseOracle(connectionString,
                x => x.MigrationsAssembly("Telemetry.Migrations.Oracle"));

            LoadAssembly(dbProvider, "Telemetry.Migrations.Oracle.dll");
            break;
        case "postgres":
            Log.Information("Using PostgresSQL database");
            options.UseNpgsql(connectionString,
                x => x.MigrationsAssembly("Telemetry.Migrations.Postgres"));

            LoadAssembly(dbProvider, "Telemetry.Migrations.Postgres.dll");
            break;
        case "sqlite":
            Log.Information("Using SQLite database");
            options.UseSqlite(connectionString,
                x => x.MigrationsAssembly("Telemetry.Migrations.Sqlite"));

            LoadAssembly(dbProvider, "Telemetry.Migrations.Sqlite.dll");
            break;
        case "mssql":
            Log.Information("Using MS SQL database");
            options.UseSqlServer(connectionString,
                x => x.MigrationsAssembly("Telemetry.Migrations.SqlServer"));

            LoadAssembly(dbProvider, "Telemetry.Migrations.SqlServer.dll");
            break;
        case "mongodb":
            Log.Information("Using MongoDB database");
            string mongoDbName = builder.Configuration.GetValue<string>("MongoDbDatabaseName") ?? "pyrevit-telemetry";
            options.UseMongoDB(connectionString, mongoDbName);

            break;
        case "mongodb_native":
            break;
        default:
            throw new NotSupportedException($"Provider {dbProvider} is not supported.");
    }
});

string? dbProviderValue = builder.Configuration.GetValue<string>("DbProvider")?.ToLower();
if (dbProviderValue?.Equals("mongodb_native") == true)
{
    string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Missing connection string");
    }

    string mongoDbName = builder.Configuration.GetValue<string>("MongoDbDatabaseName") ?? "pyrevit-telemetry";
    builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
    builder.Services.AddScoped<IApplicationDbContext>(provider =>
        new MongoNativeDbContext(provider.GetRequiredService<IMongoClient>(), mongoDbName));
}
else
{
    builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
}

builder.Services.AddSingleton<IServiceInfo, ServiceInfo>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    string basePath = AppContext.BaseDirectory;
    options.IncludeXmlComments(Path.Combine(basePath, "Telemetry.Api.xml"));

    options.SwaggerDoc("v2",
        new OpenApiInfo
        {
            Version = "v2",
            Title = "pyRevit Telemetry API",
            Summary = "API for collecting platform usage telemetry and Revit application events from pyRevit.",
            Description =
                "This API provides endpoints for collecting, storing, and accessing telemetry data related to pyRevit platform usage, including user activity, command execution, performance metrics, and events raised inside the Autodesk Revit application. It is intended for monitoring usage patterns, analyzing user interactions, diagnosing issues, and auditing application behavior.",
            Contact = new OpenApiContact {Name = "pyrevitlabs", Url = new Uri("https://www.pyrevitlabs.io/")},
            License = new OpenApiLicense
            {
                Name = "GNU GPL v3",
                Url = new Uri(
                    "https://raw.githubusercontent.com/pyrevitlabs/telemetry-server/refs/heads/main/LICENSE.md")
            }
        });
});

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(2, 0);
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "pyRevit Telemetry API v2");
    });
}

app.UseRouting();
app.UseHttpMetrics();

app.MapControllers();
app.MapMetrics();

try
{
    Log.Information("Starting migration");

    using IServiceScope scope = app.Services.CreateScope();
    ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    string? currentDbProvider = builder.Configuration.GetValue<string>("DbProvider")?.ToLower();
    if (currentDbProvider is not "mongodb" && currentDbProvider is not "mongodb_native")
    {
        await db.Database.MigrateAsync();
    }

    Log.Information("Migration completed");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Migration terminated unexpectedly");
    throw;
}

try
{
    app.Logger.LogInformation("Starting web host");
    await app.RunAsync();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Host terminated unexpectedly");
}

return;

void LoadAssembly(string provider, string assemlyName)
{
    string assemblyPath = Path.Combine(Environment.CurrentDirectory, assemlyName);
    if (File.Exists(assemblyPath))
    {
        Log.Information(
            "Load {Provider} migration assembly {AssemblyPath}", provider, assemblyPath);
        AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
    }
}