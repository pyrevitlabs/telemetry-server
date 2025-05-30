package persistence

import (
	"database/sql"
	"encoding/json"
	"fmt"
	"log"
	"reflect"
	"regexp"
	"strconv"
	"strings"

	"pyrevittelemetryserver/cli"

	_ "github.com/denisenkom/go-mssqldb"
	_ "github.com/go-sql-driver/mysql"
	_ "github.com/lib/pq"
	_ "github.com/mattn/go-sqlite3"

	uuid "github.com/satori/go.uuid"
)

type GenericSQLConnection struct {
	DatabaseConnection
}

func (w GenericSQLConnection) GetType() DBBackend {
	return w.Config.Backend
}

func (w GenericSQLConnection) GetVersion(logger *cli.Logger) string {
	db, err := openConnection(w.Config.Backend, w.Config.ConnString, logger)
	if err != nil {
		logger.Debug("error opening connection")
		return ""
	}
	defer db.Close()

	var version string
	err = db.QueryRow("select version()").Scan(&version)
	if err != nil {
		err = db.QueryRow("select @@version").Scan(&version)
		if err != nil {
			log.Fatal(err)
		}
	}
	return version
}

func (w GenericSQLConnection) GetStatus(logger *cli.Logger) ConnectionStatus {
	return ConnectionStatus{
		Status:  "pass",
		Version: w.GetVersion(logger),
	}
}

func (w GenericSQLConnection) WriteScriptTelemetryV1(logrec *ScriptTelemetryRecordV1, logger *cli.Logger) (*Result, error) {
	// generate generic sql insert query
	logger.Debug("generating query")
	query, qErr := generateScriptInsertQueryV1(w.Config.ScriptTarget, logrec, logger)
	if qErr != nil {
		return nil, qErr
	}

	return commitSQL(w.Config.Backend, w.Config.ConnString, query, logger)
}

func (w GenericSQLConnection) WriteScriptTelemetryV2(logrec *ScriptTelemetryRecordV2, logger *cli.Logger) (*Result, error) {
	// generate generic sql insert query
	logger.Debug("generating query")
	query, qErr := generateScriptInsertQueryV2(w.Config.ScriptTarget, logrec, logger)
	if qErr != nil {
		return nil, qErr
	}

	return commitSQL(w.Config.Backend, w.Config.ConnString, query, logger)
}

func (w GenericSQLConnection) WriteEventTelemetryV2(logrec *EventTelemetryRecordV2, logger *cli.Logger) (*Result, error) {
	// generate generic sql insert query
	logger.Debug("generating query")
	query, qErr := generateEventInsertQueryV2(w.Config.EventTarget, logrec, logger)
	if qErr != nil {
		return nil, qErr
	}

	return commitSQL(w.Config.Backend, w.Config.ConnString, query, logger)
}

func commitSQL(backend DBBackend, connStr string, query string, logger *cli.Logger) (*Result, error) {
	// open connection
	db, err := openConnection(backend, connStr, logger)
	if err != nil {
		logger.Debug("error opening connection")
		return nil, err
	}
	defer db.Close()

	// start transaction
	logger.Debug("opening transaction")
	tx, beginErr := db.Begin()
	if beginErr != nil {
		logger.Debug("error opening transaction")
		return nil, beginErr
	}
	defer tx.Rollback()

	// run the insert query
	logger.Debug("executing insert query")
	_, eErr := db.Exec(query)
	if eErr != nil {
		return nil, eErr
	}

	// commit transaction
	logger.Debug("commiting transaction")
	txnErr := tx.Commit()
	if txnErr != nil {
		return nil, txnErr
	}

	logger.Debug("preparing report")
	return &Result{
		Message: "successfully inserted usage record",
	}, nil
}

func openConnection(backend DBBackend, connStr string, logger *cli.Logger) (*sql.DB, error) {
	// open connection
	logger.Debug(fmt.Sprintf("opening %s connection", backend))
	cleanConnStr := connStr
	if backend == Sqlite || backend == MySql {
		cleanConnStr = strings.Replace(connStr, string(backend)+":", "", 1)
	}
	return sql.Open(string(backend), cleanConnStr)
}

func generateScriptInsertQueryV1(table string, logrec *ScriptTelemetryRecordV1, logger *cli.Logger) (string, error) {
	// read csv file and build sql insert query
	var querystr strings.Builder

	logger.Debug("generating insert query with-out headers")
	querystr.WriteString(fmt.Sprintf("INSERT INTO %s values ", table))

	// build sql data info
	logger.Debug("building insert query for data")
	datalines := make([]string, 0)

	cresults, merr := json.Marshal(logrec.CommandResults)
	if merr != nil {
		logger.Debug("error logging command results")
	}

	// create record based on schema
	var record []string

	// generate record id, panic if error
	recordId := uuid.NewV4()

	re := regexp.MustCompile(`(\d+:\d+:\d+)`)
	record = []string{
		recordId.String(),
		logrec.Date,
		re.FindString(logrec.Time),
		logrec.UserName,
		logrec.RevitVersion,
		logrec.RevitBuild,
		logrec.SessionId,
		logrec.PyRevitVersion,
		strconv.FormatBool(logrec.IsDebugMode),
		strconv.FormatBool(logrec.IsConfigMode),
		logrec.CommandName,
		logrec.BundleName,
		logrec.ExtensionName,
		logrec.CommandUniqueName,
		strconv.Itoa(logrec.ResultCode),
		string(cresults),
		logrec.ScriptPath,
		logrec.TraceInfo.EngineInfo.Version,
		logrec.TraceInfo.IronPythonTraceDump,
		logrec.TraceInfo.CLRTraceDump,
	}

	datalines = append(datalines, ToSql(&record, true))

	// add csv records to query string
	all_datalines := strings.Join(datalines, ", ")
	logger.Trace(all_datalines)
	querystr.WriteString(all_datalines)
	querystr.WriteString(";\n")
	logger.Debug("building query completed")

	// execute query
	full_query := querystr.String()
	logger.Trace(full_query)
	return full_query, nil
}

func generateScriptInsertQueryV2(table string, logrec *ScriptTelemetryRecordV2, logger *cli.Logger) (string, error) {
	// read csv file and build sql insert query
	var querystr strings.Builder

	logger.Debug("generating insert query with-out headers")
	querystr.WriteString(fmt.Sprintf("INSERT INTO %s values ", table))

	// build sql data info
	logger.Debug("building insert query for data")
	datalines := make([]string, 0)

	// marshal json data
	engineCfgs, merr := json.Marshal(logrec.TraceInfo.EngineInfo.Configs)
	if merr != nil {
		logger.Debug("error logging engine configs")
	}

	// marshal json data
	cresults, merr := json.Marshal(logrec.CommandResults)
	if merr != nil {
		logger.Debug("error logging command results")
	}

	// create record based on schema
	var record []string

	// generate record id, panic if error
	recordId := uuid.NewV4()

	record = []string{
		recordId.String(),
		logrec.TimeStamp,
		logrec.UserName,
		logrec.HostUserName,
		logrec.RevitVersion,
		logrec.RevitBuild,
		logrec.SessionId,
		logrec.PyRevitVersion,
		logrec.Clone,
		strconv.FormatBool(logrec.IsDebugMode),
		strconv.FormatBool(logrec.IsConfigMode),
		strconv.FormatBool(logrec.IsExecFromGUI),
		logrec.ExecId,
		logrec.ExecTimeStamp,
		logrec.CommandName,
		logrec.BundleName,
		logrec.ExtensionName,
		logrec.CommandUniqueName,
		logrec.DocumentName,
		logrec.DocumentPath,
		strconv.Itoa(logrec.ResultCode),
		string(cresults),
		logrec.ScriptPath,
		logrec.TraceInfo.EngineInfo.Type,
		logrec.TraceInfo.EngineInfo.Version,
		strings.Join(logrec.TraceInfo.EngineInfo.SysPaths, ";"),
		string(engineCfgs),
		logrec.TraceInfo.Message,
	}
	datalines = append(datalines, ToSql(&record, true))

	// add csv records to query string
	all_datalines := strings.Join(datalines, ", ")
	logger.Trace(all_datalines)
	querystr.WriteString(all_datalines)
	querystr.WriteString(";\n")
	logger.Debug("building query completed")

	// execute query
	full_query := querystr.String()
	logger.Trace(full_query)
	return full_query, nil
}

func generateEventInsertQueryV2(table string, logrec *EventTelemetryRecordV2, logger *cli.Logger) (string, error) {
	// read csv file and build sql insert query
	var querystr strings.Builder

	logger.Debug("generating insert query with-out headers")
	querystr.WriteString(fmt.Sprintf("INSERT INTO %s values ", table))

	// build sql data info
	logger.Debug("building insert query for data")
	datalines := make([]string, 0)

	// marshal json data
	cresults, merr := json.Marshal(logrec.EventArgs)
	if merr != nil {
		logger.Debug("error logging command results")
	}

	// create record based on schema
	var record []string

	// generate record id, panic if error
	recordId := uuid.NewV4()

	record = []string{
		recordId.String(),
		logrec.TimeStamp,
		logrec.HandlerId,
		logrec.EventType,
		string(cresults),
		logrec.UserName,
		logrec.HostUserName,
		logrec.RevitVersion,
		logrec.RevitBuild,
		strconv.FormatBool(logrec.Cancellable),
		strconv.FormatBool(logrec.Cancelled),
		strconv.Itoa(logrec.DocumentId),
		logrec.DocumentType,
		logrec.DocumentTemplate,
		logrec.DocumentName,
		logrec.DocumentPath,
		logrec.ProjectNumber,
		logrec.ProjectName,
	}
	datalines = append(datalines, ToSql(&record, true))

	// add csv records to query string
	all_datalines := strings.Join(datalines, ", ")
	logger.Trace(all_datalines)
	querystr.WriteString(all_datalines)
	querystr.WriteString(";\n")
	logger.Debug("building query completed")

	// execute query
	full_query := querystr.String()
	logger.Trace(full_query)
	return full_query, nil
}

// Read methods for SQL databases
func (w GenericSQLConnection) ReadScriptTelemetryV1(limit int, offset int, logger *cli.Logger) ([]ScriptTelemetryRecordV1, error) {
	return readSQL[ScriptTelemetryRecordV1](w.Config.Backend, w.Config.ConnString, w.Config.ScriptTarget, nil, limit, offset, logger)
}

func (w GenericSQLConnection) ReadScriptTelemetryV2(limit int, offset int, logger *cli.Logger) ([]ScriptTelemetryRecordV2, error) {
	return readSQL[ScriptTelemetryRecordV2](w.Config.Backend, w.Config.ConnString, w.Config.ScriptTarget, nil, limit, offset, logger)
}

func (w GenericSQLConnection) ReadEventTelemetryV2(limit int, offset int, logger *cli.Logger) ([]EventTelemetryRecordV2, error) {
	return readSQL[EventTelemetryRecordV2](w.Config.Backend, w.Config.ConnString, w.Config.EventTarget, nil, limit, offset, logger)
}

// Search methods for SQL databases
func (w GenericSQLConnection) SearchScriptTelemetryV1(query map[string]interface{}, limit int, offset int, logger *cli.Logger) ([]ScriptTelemetryRecordV1, error) {
	return readSQL[ScriptTelemetryRecordV1](w.Config.Backend, w.Config.ConnString, w.Config.ScriptTarget, query, limit, offset, logger)
}

func (w GenericSQLConnection) SearchScriptTelemetryV2(query map[string]interface{}, limit int, offset int, logger *cli.Logger) ([]ScriptTelemetryRecordV2, error) {
	return readSQL[ScriptTelemetryRecordV2](w.Config.Backend, w.Config.ConnString, w.Config.ScriptTarget, query, limit, offset, logger)
}

func (w GenericSQLConnection) SearchEventTelemetryV2(query map[string]interface{}, limit int, offset int, logger *cli.Logger) ([]EventTelemetryRecordV2, error) {
	return readSQL[EventTelemetryRecordV2](w.Config.Backend, w.Config.ConnString, w.Config.EventTarget, query, limit, offset, logger)
}

// Generic read function for SQL databases
func readSQL[T any](backend DBBackend, connStr string, table string, query map[string]interface{}, limit int, offset int, logger *cli.Logger) ([]T, error) {
	// Open connection
	db, err := openConnection(backend, connStr, logger)
	if err != nil {
		return nil, err
	}
	defer db.Close()

	// Build query
	var sqlQuery strings.Builder
	sqlQuery.WriteString(fmt.Sprintf("SELECT * FROM %s", table))

	// Add WHERE clause if query parameters provided
	args := make([]interface{}, 0)
	if query != nil {
		sqlQuery.WriteString(" WHERE ")
		conditions := make([]string, 0)
		for key, value := range query {
			conditions = append(conditions, fmt.Sprintf("%s = ?", key))
			args = append(args, value)
		}
		sqlQuery.WriteString(strings.Join(conditions, " AND "))
	}

	// Add ORDER BY and LIMIT/OFFSET
	sqlQuery.WriteString(" ORDER BY timestamp DESC")
	sqlQuery.WriteString(fmt.Sprintf(" LIMIT %d OFFSET %d", limit, offset))

	// Execute query
	rows, err := db.Query(sqlQuery.String(), args...)
	if err != nil {
		return nil, err
	}
	defer rows.Close()

	// Scan results
	var results []T
	for rows.Next() {
		var record T
		// Use reflection to scan into struct fields
		val := reflect.ValueOf(&record).Elem()
		fields := make([]interface{}, val.NumField())
		for i := 0; i < val.NumField(); i++ {
			fields[i] = val.Field(i).Addr().Interface()
		}
		if err := rows.Scan(fields...); err != nil {
			return nil, err
		}
		results = append(results, record)
	}

	if err = rows.Err(); err != nil {
		return nil, err
	}

	return results, nil
}
