package server

import (
	"encoding/json"
	"fmt"
	"net/http"
	"strconv"

	"pyrevittelemetryserver/cli"
	"pyrevittelemetryserver/persistence"

	"github.com/gorilla/mux"
)

func dumpEventAndRespond(logrec interface{}, w http.ResponseWriter, logger *cli.Logger) {
	jsonData, responseDataErr := json.Marshal(logrec)
	if responseDataErr == nil {
		jsonString := string(jsonData)
		if logger.PrintTrace {
			logger.Trace(jsonString)
		}
		w.Header().Set("Content-Type", "application/json")
		_, responseErr := w.Write([]byte(jsonString))
		if responseErr != nil {
			logger.Debug(responseErr)
		}
	} else {
		logger.Debug(responseDataErr)
	}
}

func RouteEvents(router *mux.Router, opts *cli.Options, dbConn persistence.Connection, logger *cli.Logger) {
	// POST events/
	// create new script telemetry record
	// https://stackoverflow.com/a/26212073
	router.HandleFunc("/api/v2/events/", func(w http.ResponseWriter, r *http.Request) {
		// parse given json data into a new record
		logrec := persistence.EventTelemetryRecordV2{}
		decodeErr := json.NewDecoder(r.Body).Decode(&logrec)
		if decodeErr != nil {
			logger.Debug(decodeErr)
			return
		}

		err := logrec.Validate()
		if err != nil {
			// log error
			logrec.PrintRecordInfo(logger, fmt.Sprintf("[ {r}%s{!} ]", err.Error()))
			// respond with error
			w.WriteHeader(http.StatusBadRequest)
			respondError(err, w, logger)
		} else {
			// now write to db
			_, dbWriteErr := dbConn.WriteEventTelemetryV2(&logrec, logger)
			if dbWriteErr != nil {
				logger.Debug(dbWriteErr)
				logrec.PrintRecordInfo(logger, fmt.Sprintf("[ {r}%s{!} ]", dbWriteErr))
			} else {
				logrec.PrintRecordInfo(logger, OkMessage)
			}
			// respond with the created data
			dumpEventAndRespond(logrec, w, logger)
		}

	}).Methods("POST")

	// GET events/
	// get recorded telemetry record
	router.HandleFunc("/api/v2/events/", func(w http.ResponseWriter, r *http.Request) {
		// Parse query parameters
		limit := 100 // Default limit
		offset := 0  // Default offset
		if limitStr := r.URL.Query().Get("limit"); limitStr != "" {
			if l, err := strconv.Atoi(limitStr); err == nil && l > 0 {
				limit = l
			}
		}
		if offsetStr := r.URL.Query().Get("offset"); offsetStr != "" {
			if o, err := strconv.Atoi(offsetStr); err == nil && o >= 0 {
				offset = o
			}
		}

		// Parse search query if provided
		var searchQuery map[string]interface{}
		if searchStr := r.URL.Query().Get("q"); searchStr != "" {
			if err := json.Unmarshal([]byte(searchStr), &searchQuery); err != nil {
				w.WriteHeader(http.StatusBadRequest)
				respondError(fmt.Errorf("invalid search query: %v", err), w, logger)
				return
			}
		}

		// Get records
		var records []persistence.EventTelemetryRecordV2
		var err error
		if searchQuery != nil {
			records, err = dbConn.SearchEventTelemetryV2(searchQuery, limit, offset, logger)
		} else {
			records, err = dbConn.ReadEventTelemetryV2(limit, offset, logger)
		}

		if err != nil {
			logger.Debug(err)
			w.WriteHeader(http.StatusInternalServerError)
			respondError(err, w, logger)
			return
		}

		// Write response
		w.Header().Set("Content-Type", "application/json")
		jsonData, err := json.Marshal(records)
		if err != nil {
			logger.Debug(err)
			w.WriteHeader(http.StatusInternalServerError)
			respondError(err, w, logger)
			return
		}

		if _, err := w.Write(jsonData); err != nil {
			logger.Debug(err)
		}
	}).Methods("GET")
}
