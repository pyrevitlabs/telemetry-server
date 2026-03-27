FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

# fix cannot load library libgssapi_krb5.so.2 (postgres)
RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

USER $APP_UID
WORKDIR /app

EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/Telemetry.Api/Telemetry.Api.csproj", "Telemetry.Api/"]
COPY ["src/Telemetry.Migrations.Oracle/Telemetry.Migrations.Oracle.csproj", "Telemetry.Migrations.Oracle/"]
COPY ["src/Telemetry.Migrations.Postgres/Telemetry.Migrations.Postgres.csproj", "Telemetry.Migrations.Postgres/"]
COPY ["src/Telemetry.Migrations.Sqlite/Telemetry.Migrations.Sqlite.csproj", "Telemetry.Migrations.Sqlite/"]
COPY ["src/Telemetry.Migrations.SqlServer/Telemetry.Migrations.SqlServer.csproj", "Telemetry.Migrations.SqlServer/"]

RUN dotnet restore "./Telemetry.Api/Telemetry.Api.csproj"
RUN dotnet restore "./Telemetry.Migrations.Oracle/Telemetry.Migrations.Oracle.csproj"
RUN dotnet restore "./Telemetry.Migrations.Postgres/Telemetry.Migrations.Postgres.csproj"
RUN dotnet restore "./Telemetry.Migrations.Sqlite/Telemetry.Migrations.Sqlite.csproj"
RUN dotnet restore "./Telemetry.Migrations.SqlServer/Telemetry.Migrations.SqlServer.csproj"

COPY . .
RUN dotnet build "./src/Telemetry.Api/Telemetry.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet build "./src/Telemetry.Migrations.Oracle/Telemetry.Migrations.Oracle.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet build "./src/Telemetry.Migrations.Postgres/Telemetry.Migrations.Postgres.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet build "./src/Telemetry.Migrations.Sqlite/Telemetry.Migrations.Sqlite.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet build "./src/Telemetry.Migrations.SqlServer/Telemetry.Migrations.SqlServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./src/Telemetry.Api/Telemetry.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
RUN dotnet publish "./src/Telemetry.Migrations.Oracle/Telemetry.Migrations.Oracle.csproj" -c $BUILD_CONFIGURATION -o /app/migrations/oracle
RUN dotnet publish "./src/Telemetry.Migrations.Postgres/Telemetry.Migrations.Postgres.csproj" -c $BUILD_CONFIGURATION -o /app/migrations/postgres
RUN dotnet publish "./src/Telemetry.Migrations.Sqlite/Telemetry.Migrations.Sqlite.csproj" -c $BUILD_CONFIGURATION -o /app/migrations/sqlite
RUN dotnet publish "./src/Telemetry.Migrations.SqlServer/Telemetry.Migrations.SqlServer.csproj" -c $BUILD_CONFIGURATION -o /app/migrations/sqlserver

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
COPY --from=publish /app/migrations/oracle/Telemetry.Migrations.Oracle.dll .
COPY --from=publish /app/migrations/postgres/Telemetry.Migrations.Postgres.dll .
COPY --from=publish /app/migrations/sqlite/Telemetry.Migrations.Sqlite.dll .
COPY --from=publish /app/migrations/sqlserver/Telemetry.Migrations.SqlServer.dll .

ENTRYPOINT ["dotnet", "Telemetry.Api.dll"]
