# pyRevit Telemetry Server

## Quick Start

```sh
# Build the server
go build -o pyrevit-telemetryserver

# Run with environment variables
PYREVT_TELEMETRY_DB_CONNSTRING="mongodb://localhost:27017/atoma-test" \
PYREVT_TELEMETRY_SCRIPTS_TABLE="scripts" \
PYREVT_TELEMETRY_EVENTS_TABLE="events" \
./pyrevit-telemetryserver --port=8080
```

## Environment Variables

| Variable                              | Description                                 | Example                                 |
|----------------------------------------|---------------------------------------------|-----------------------------------------|
| PYREVT_TELEMETRY_DB_BACKEND            | Database backend (`mongo`, `postgres`, etc) | mongo                                   |
| PYREVT_TELEMETRY_DB_CONNSTRING         | Database connection string                  | mongodb://localhost:27017/atoma-test    |
| PYREVT_TELEMETRY_SCRIPTS_TABLE         | Name of the scripts table/collection        | scripts                                 |
| PYREVT_TELEMETRY_EVENTS_TABLE          | Name of the events table/collection         | events                                  |
| PYREVT_TELEMETRY_PORT                  | Port to run the server on                   | 8080                                    |
| PYREVT_TELEMETRY_DEBUG                 | Enable debug logging (`true`/`false`)       | true                                    |
| PYREVT_TELEMETRY_TRACE                 | Enable trace logging (`true`/`false`)       | false                                   |

> All CLI options can be set via environment variables. CLI flags override environment variables.

## API Endpoints

- `GET /api/v1/status` — Health check
- `GET /api/v1/scripts/` — List v1 script telemetry
- `POST /api/v1/scripts/` — Submit v1 script telemetry
- `GET /api/v2/scripts/` — List v2 script telemetry
- `POST /api/v2/scripts/` — Submit v2 script telemetry
- `GET /api/v2/events/` — List v2 event telemetry
- `POST /api/v2/events/` — Submit v2 event telemetry

## Example: Submit Script Telemetry

```sh
curl -X POST -H "Content-Type: application/json" \
  -d '{"date":"2024-03-30","time":"08:45:00", ... }' \
  http://localhost:8080/api/v1/scripts/
```

## Docker Usage

### Build and Run with Docker Compose

```sh
docker-compose up --build
```

This will start both DB and the telemetry server. The server will be available at `http://localhost:8080`. 