# pyRevit Telemetry Server

A Go HTTP server that receives telemetry from [pyRevit](https://github.com/pyrevitlabs/pyRevit) and stores it in a
database. It ships as a single binary with no external runtime dependencies (except the database driver and, when using
SQLite, a C toolchain for CGO).

The server is useful for teams and companies that centrally track pyRevit usage: which tools are run, in what
environment, with what errors, as well as Revit and pyRevit events.

## Report types

| Type                          | Description                                                                                                                             |
|-------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------|
| **Script Execution Reports**  | Each time a pyRevit tool runs, a report is sent with the tool, environment, result, engine errors, and any custom data from the script. |
| **Application Event Reports** | Revit events (for example, document opened) that pyRevit is configured to listen to.                                                    |
| **Custom Application Events** | pyRevit-specific events with telemetry data.                                                                                            |

## Quick start

1. Set up a database (locally or in the cloud) to store records.
2. Build or copy the server binary onto a host reachable from machines running Revit/pyRevit.
3. Start the server with the database URI, port, and table/collection names.
4. Point pyRevit telemetry settings on clients at the server URL.
5. Optionally extend your pyRevit scripts with extra fields in reports.

## Requirements

- **Go** 1.24+ (see `go.mod`)
- A network-reachable **database** with tables/collections already created (the server does not create schema—it only
  `INSERT`s)
- For **SQLite3**: **CGO** enabled and a C toolchain installed (on macOS/Linux: Xcode CLT or `build-essential`)

## Building

From the repository root:

```bash
go build -o bin/pyrevit-telemetryserver .
```

On Windows (same name as in the pyRevit distribution):

```bash
go build -o bin/pyrevit-telemetryserver.exe .
```

Cross-compile for Windows from macOS/Linux:

```bash
GOOS=windows GOARCH=amd64 go build -o bin/pyrevit-telemetryserver.exe .
```

Check the built binary version:

```bash
./bin/pyrevit-telemetryserver --version
```

Current version in code: **0.19** (`cli/usage.go`).

### Dependencies

```bash
go mod download
```

Database drivers are wired through `database/sql` and the official MongoDB driver:

| Database             | Driver                              |
|----------------------|-------------------------------------|
| PostgreSQL           | `github.com/lib/pq`                 |
| MySQL                | `github.com/go-sql-driver/mysql`    |
| Microsoft SQL Server | `github.com/denisenkom/go-mssqldb`  |
| SQLite3              | `github.com/mattn/go-sqlite3` (CGO) |
| MongoDB              | `go.mongodb.org/mongo-driver`       |

## Running

### Syntax

```
pyrevit-telemetryserver <db_uri> [--scripts=<table>] [--events=<table>] --port=<port> [--https] [--debug] [--trace]
```

| Option            | Description                                                                           |
|-------------------|---------------------------------------------------------------------------------------|
| `<db_uri>`        | Database connection string (required positional argument)                             |
| `--scripts`       | Table or collection for script execution logs                                         |
| `--events`        | Table or collection for application events                                            |
| `--port`          | HTTP(S) server TCP port (**required**)                                                |
| `--https`         | TLS; expects `./<binary_name>.crt` and `./<binary_name>.key` in the working directory |
| `--debug`         | Debug messages                                                                        |
| `--trace`         | Verbose output (including JSON records and SQL)                                       |
| `-h`, `--help`    | Help                                                                                  |
| `-V`, `--version` | Version                                                                               |

Routes are registered only for configured targets: without `--scripts`, the scripts API is unavailable; without
`--events`, the events API is unavailable. The status endpoint is always available.

### Database URI examples

Encode special characters in username and password
using [percent-encoding](https://en.wikipedia.org/wiki/Percent-encoding) (for example, `#` → `%23`).

```
# PostgreSQL
postgres://user:pass@data.example.com/mydb

# MongoDB
mongodb://user:pass@localhost:27017/mydb

# MySQL
mysql:user:pass@tcp(localhost:3306)/tests

# Microsoft SQL Server
sqlserver://user:pass@my-db.database.windows.net?database=mydb

# SQLite3 (file)
sqlite3:data.db
```

### Run examples

PostgreSQL — scripts and events:

```bash
./bin/pyrevit-telemetryserver \
  "postgres://user:pass@data.example.com/mydb" \
  --scripts=pyrevitlogs \
  --events=appevents \
  --port=8080 \
  --debug
```

Scripts only (MongoDB):

```bash
./bin/pyrevit-telemetryserver \
  "mongodb://user:pass@localhost:27017/mydb" \
  --scripts=pyrevitlogs \
  --port=8080
```

SQLite for local development:

```bash
CGO_ENABLED=1 ./bin/pyrevit-telemetryserver sqlite3:./data.db --scripts=logs --port=8080
```

Credentials via environment variables (the shell expands them in the URI):

```bash
export USER=myuser PASS=mypass
./bin/pyrevit-telemetryserver "mongodb://${USER}:${PASS}@data.example.com:27017/mydb" --scripts=logs --port=8080
```

On successful start you should see:

```
Server listening on 8080...
```

**Networking:** open the chosen TCP port in the firewall. Clients configure the server host and port in pyRevit
telemetry settings.

**Multiple instances:** you can run several processes on different ports and with different databases or tables (for
example, separate events and scripts).

### HTTPS

```bash
./bin/pyrevit-telemetryserver "postgres://..." --scripts=logs --port=8443 --https
```

The working directory must contain, for example:

- `pyrevit-telemetryserver.crt`
- `pyrevit-telemetryserver.key`

(file names match the executable name without extension).

## REST API

Base URL: `http(s)://<host>:<port>`

| Method | Path               | Purpose                            |
|--------|--------------------|------------------------------------|
| `POST` | `/api/v1/scripts/` | Write script telemetry (schema v1) |
| `POST` | `/api/v2/scripts/` | Write script telemetry (schema v2) |
| `POST` | `/api/v2/events/`  | Write application event (v2)       |
| `GET`  | `/api/v1/status`   | Health check (JSON)                |
| `GET`  | `/api/v2/status`   | Health check (JSON)                |

Request bodies use `application/json`. Validation errors return `400 Bad Request` with an error message. On successful
write, the response body is JSON of the accepted record.

`GET` endpoints for `/api/v*/scripts/` and `/api/v2/events/` are reserved but not implemented yet.

For field formats, see pyRevit documentation (Telemetry / REST API) and types in `persistence/models.go`.

### Health check

`GET /api/v1/status` and `GET /api/v2/status` return JSON aligned with
the [health check draft](https://inadarei.github.io/rfc-healthcheck/): service status, version, `serviceid` (instance
UUID), and database connectivity check.

## pyRevit configuration

On Revit clients, set this server’s address in pyRevit telemetry settings (host + port, and HTTPS if used). Official
pyRevit docs describe enabling telemetry and report formats.

In the pyRevit ecosystem, server sources historically lived under `dev/pyRevitTelemetryServer`; this repository is a
standalone build of the same service.

## Project layout

```
.
├── main.go              # entry: CLI → persistence → HTTP
├── cli/                 # arguments, logging, help
├── persistence/         # URI, connections, models, SQL/MongoDB writes
└── server/              # Gorilla mux routes and handlers
```

Go module: `pyrevittelemetryserver`.

## Troubleshooting

| Symptom                   | What to check                                                           |
|---------------------------|-------------------------------------------------------------------------|
| `db is not yet supported` | URI prefix: `postgres:`, `mongodb:`, `mysql:`, `sqlserver:`, `sqlite3:` |
| Error with `sqlite3:`     | `CGO_ENABLED=1`, C compiler installed                                   |
| Clients cannot connect    | Firewall, host DNS/IP, `--port`                                         |
| `400 Bad Request`         | JSON body failed validation (use `--trace` for details)                 |
| Database write error      | Table/collection exists, permissions, correct URI                       |

## License and origin

The server was built for the pyRevit ecosystem (eirannejad, pyRevitLabs). When deploying in your organization, follow
policies for storing personal and project data from Revit telemetry.
