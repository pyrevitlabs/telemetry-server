package cli

import (
	"os"
	"path/filepath"
	"strconv"
	"strings"

	"github.com/docopt/docopt-go"
)

// Environment variable support:
//   PYREVT_TELEMETRY_DB_CONNSTRING
//   PYREVT_TELEMETRY_SCRIPTS_TABLE
//   PYREVT_TELEMETRY_EVENTS_TABLE
//   PYREVT_TELEMETRY_PORT
//   PYREVT_TELEMETRY_HTTPS
//   PYREVT_TELEMETRY_DEBUG
//   PYREVT_TELEMETRY_TRACE
//   PYREVT_TELEMETRY_EXENAME
//   PYREVT_TELEMETRY_VERSION

type Options struct {
	ExeName      string `json:"exe_name"`
	Version      string `json:"version"`
	Opts         *docopt.Opts
	ConnString   string `json:"connection_string"`
	ScriptsTable string `json:"script_table"`
	EventsTable  string `json:"events_table"`
	Port         int    `json:"server_port"`
	Https        bool   `json:"https"`
	Debug        bool   `json:"debug_mode"`
	Trace        bool   `json:"trace_mode"`
}

func getExeName() string {
	return strings.TrimSuffix(
		filepath.Base(os.Args[0]),
		filepath.Ext(os.Args[0]),
	)
}

func NewOptions() *Options {
	argv := os.Args[1:]

	parser := &docopt.Parser{
		HelpHandler: printHelpAndExit,
	}

	opts, _ := parser.ParseArgs(help, argv, version)

	connString, _ := opts.String("<db_uri>")
	scriptTable, _ := opts.String("--scripts")
	eventTable, _ := opts.String("--events")
	port, _ := opts.Int("--port")
	https, _ := opts.Bool("--https")

	debug, _ := opts.Bool("--debug")
	trace, _ := opts.Bool("--trace")

	// Environment variable fallback
	if connString == "" {
		connString = os.Getenv("PYREVT_TELEMETRY_DB_CONNSTRING")
	}
	if scriptTable == "" {
		scriptTable = os.Getenv("PYREVT_TELEMETRY_SCRIPTS_TABLE")
	}
	if eventTable == "" {
		eventTable = os.Getenv("PYREVT_TELEMETRY_EVENTS_TABLE")
	}
	if port == 0 {
		if portStr := os.Getenv("PYREVT_TELEMETRY_PORT"); portStr != "" {
			if p, err := strconv.Atoi(portStr); err == nil {
				port = p
			}
		}
	}
	if !https {
		if httpsStr := os.Getenv("PYREVT_TELEMETRY_HTTPS"); httpsStr != "" {
			if httpsStr == "1" || strings.ToLower(httpsStr) == "true" {
				https = true
			}
		}
	}
	if !debug {
		if debugStr := os.Getenv("PYREVT_TELEMETRY_DEBUG"); debugStr != "" {
			if debugStr == "1" || strings.ToLower(debugStr) == "true" {
				debug = true
			}
		}
	}
	if !trace {
		if traceStr := os.Getenv("PYREVT_TELEMETRY_TRACE"); traceStr != "" {
			if traceStr == "1" || strings.ToLower(traceStr) == "true" {
				trace = true
			}
		}
	}

	exeName := getExeName()
	if envExe := os.Getenv("PYREVT_TELEMETRY_EXENAME"); envExe != "" {
		exeName = envExe
	}
	ver := version
	if envVer := os.Getenv("PYREVT_TELEMETRY_VERSION"); envVer != "" {
		ver = envVer
	}

	return &Options{
		ExeName:      exeName,
		Version:      ver,
		Opts:         &opts,
		ConnString:   connString,
		ScriptsTable: scriptTable,
		EventsTable:  eventTable,
		Port:         port,
		Https:        https,
		Debug:        debug,
		Trace:        trace,
	}
}
