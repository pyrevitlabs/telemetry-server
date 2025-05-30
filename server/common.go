package server

import (
	"net/http"

	"pyrevittelemetryserver/cli"
)

const OkMessage = "[ {g}OK{!} ]"

func respondError(err error, w http.ResponseWriter, logger *cli.Logger) {
	message := err.Error()
	logger.Debug("validation error: ", message)
	_, responseErr := w.Write([]byte(message))
	if responseErr != nil {
		logger.Debug(responseErr)
	}
}
