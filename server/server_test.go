package server

import (
	"net/http"
	"net/http/httptest"
	"pyrevittelemetryserver/cli"
	"pyrevittelemetryserver/persistence"
	"strings"
	"testing"
)

type mockDB struct{ persistence.Connection }
type mockLogger struct{ cli.Logger }

func TestStatusEndpoint(t *testing.T) {
	opts := &cli.Options{
		ScriptsTable: "scripts",
		EventsTable:  "events",
	}
	db := &mockDB{}
	logger := &cli.Logger{}

	router := NewRouter(opts, db, logger)
	req := httptest.NewRequest("GET", "/api/v1/status", nil)
	w := httptest.NewRecorder()
	router.ServeHTTP(w, req)

	if w.Code != http.StatusOK {
		t.Fatalf("expected 200 OK, got %d", w.Code)
	}
	if w.Body.Len() == 0 {
		t.Fatalf("expected non-empty body")
	}
}

func (m *mockDB) WriteScriptTelemetryV1(logrec *persistence.ScriptTelemetryRecordV1, logger *cli.Logger) (*persistence.Result, error) {
	return &persistence.Result{ResultCode: 0, Message: "ok"}, nil
}
func (m *mockDB) WriteScriptTelemetryV2(logrec *persistence.ScriptTelemetryRecordV2, logger *cli.Logger) (*persistence.Result, error) {
	return &persistence.Result{ResultCode: 0, Message: "ok"}, nil
}
func (m *mockDB) WriteEventTelemetryV2(logrec *persistence.EventTelemetryRecordV2, logger *cli.Logger) (*persistence.Result, error) {
	return &persistence.Result{ResultCode: 0, Message: "ok"}, nil
}
func (m *mockDB) ReadScriptTelemetryV1(limit int, offset int, logger *cli.Logger) ([]persistence.ScriptTelemetryRecordV1, error) {
	return []persistence.ScriptTelemetryRecordV1{}, nil
}
func (m *mockDB) ReadScriptTelemetryV2(limit int, offset int, logger *cli.Logger) ([]persistence.ScriptTelemetryRecordV2, error) {
	return []persistence.ScriptTelemetryRecordV2{}, nil
}
func (m *mockDB) ReadEventTelemetryV2(limit int, offset int, logger *cli.Logger) ([]persistence.EventTelemetryRecordV2, error) {
	return []persistence.EventTelemetryRecordV2{}, nil
}
func (m *mockDB) GetType() persistence.DBBackend {
	return "mock"
}
func (m *mockDB) GetStatus(logger *cli.Logger) persistence.ConnectionStatus {
	return persistence.ConnectionStatus{
		Status:  "pass",
		Version: "test",
		Output:  "mock",
	}
}

func TestScriptV1Endpoints(t *testing.T) {
	router := NewRouter(&cli.Options{ScriptsTable: "scripts", EventsTable: "events"}, &mockDB{}, &cli.Logger{})
	// Valid payload
	payload := `{"date":"2024-03-30","time":"08:45:00","username":"user","revit":"2021","revitbuild":"20240330_1234(x64)","sessionid":"b3b1a2e0-4b5c-4d2a-8c3e-2b1a2e04b5c4","pyrevit":"4.8","debug":false,"config":false,"commandname":"cmd","commanduniquename":"cmd.unique","commandbundle":"bundle","commandextension":"ext","resultcode":0,"commandresults":{},"scriptpath":"/path/to/script","trace":{"engine":{"version":"1.0","syspath":[]},"ipy":"","clr":""}}`
	req := httptest.NewRequest("POST", "/api/v1/scripts/", strings.NewReader(payload))
	req.Header.Set("Content-Type", "application/json")
	w := httptest.NewRecorder()
	router.ServeHTTP(w, req)
	if w.Code != http.StatusOK {
		t.Fatalf("expected 200 OK, got %d", w.Code)
	}
	if w.Body.Len() == 0 {
		t.Fatalf("expected non-empty body")
	}
	// GET
	req = httptest.NewRequest("GET", "/api/v1/scripts/", nil)
	w = httptest.NewRecorder()
	router.ServeHTTP(w, req)
	if w.Code != http.StatusOK {
		t.Fatalf("expected 200 OK, got %d", w.Code)
	}
}

func TestScriptV2Endpoints(t *testing.T) {
	router := NewRouter(&cli.Options{ScriptsTable: "scripts", EventsTable: "events"}, &mockDB{}, &cli.Logger{})
	// Valid payload
	payload := `{"meta":{"schema":"2.0"},"timestamp":"2024-03-30T08:45:00Z","username":"user","host_user":"host","revit":"2021","revitbuild":"20240330_1234(x64)","sessionid":"b3b1a2e0-4b5c-4d2a-8c3e-2b1a2e04b5c4","pyrevit":"4.8","clone":"main","debug":false,"config":false,"from_gui":false,"exec_id":"execid","exec_timestamp":"2024-03-30T08:45:00Z","commandname":"cmd","commanduniquename":"cmd.unique","commandbundle":"bundle","commandextension":"ext","docname":"doc","docpath":"/path/to/doc","resultcode":0,"commandresults":{},"scriptpath":"/path/to/script","trace":{"engine":{"type":"ironpython","version":"1.0","syspath":[],"configs":{}},"message":""}}`
	req := httptest.NewRequest("POST", "/api/v2/scripts/", strings.NewReader(payload))
	req.Header.Set("Content-Type", "application/json")
	w := httptest.NewRecorder()
	router.ServeHTTP(w, req)
	if w.Code != http.StatusOK {
		t.Fatalf("expected 200 OK, got %d", w.Code)
	}
	if w.Body.Len() == 0 {
		t.Fatalf("expected non-empty body")
	}
	// GET
	req = httptest.NewRequest("GET", "/api/v2/scripts/", nil)
	w = httptest.NewRecorder()
	router.ServeHTTP(w, req)
	if w.Code != http.StatusOK {
		t.Fatalf("expected 200 OK, got %d", w.Code)
	}
}

func TestEventV2Endpoints(t *testing.T) {
	router := NewRouter(&cli.Options{ScriptsTable: "scripts", EventsTable: "events"}, &mockDB{}, &cli.Logger{})
	// Valid payload
	payload := `{"meta":{"schema":"2.0"},"timestamp":"2024-03-30T08:45:00Z","handler_id":"handler","type":"eventtype","args":{},"username":"user","host_user":"host","revit":"2021","revitbuild":"20240330_1234(x64)","cancellable":false,"cancelled":false,"docid":1,"doctype":"type","doctemplate":"template","docname":"doc","docpath":"/path/to/doc","projectnum":"123","projectname":"proj"}`
	req := httptest.NewRequest("POST", "/api/v2/events/", strings.NewReader(payload))
	req.Header.Set("Content-Type", "application/json")
	w := httptest.NewRecorder()
	router.ServeHTTP(w, req)
	if w.Code != http.StatusOK {
		t.Fatalf("expected 200 OK, got %d", w.Code)
	}
	if w.Body.Len() == 0 {
		t.Fatalf("expected non-empty body")
	}
	// GET
	req = httptest.NewRequest("GET", "/api/v2/events/", nil)
	w = httptest.NewRecorder()
	router.ServeHTTP(w, req)
	if w.Code != http.StatusOK {
		t.Fatalf("expected 200 OK, got %d", w.Code)
	}
}
