package persistence

import (
	"context"
	"fmt"
	"pyrevittelemetryserver/cli"
	"time"

	_ "github.com/lib/pq"
	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
	"go.mongodb.org/mongo-driver/mongo/readpref"
	"go.mongodb.org/mongo-driver/x/mongo/driver/connstring"
)

type MongoDBConnection struct {
	DatabaseConnection
}

func (w MongoDBConnection) GetType() DBBackend {
	return w.Config.Backend
}

func (w MongoDBConnection) GetVersion(logger *cli.Logger) string {
	// parse and grab database name from uri
	logger.Debug("grabbing db name from connection string")
	ctx, cancel := context.WithTimeout(context.Background(), 10*time.Second)
	defer cancel()

	logger.Debug("opening mongodb session")
	client, err := mongo.Connect(ctx, options.Client().ApplyURI(w.Config.ConnString))

	defer func() {
		if err = client.Disconnect(ctx); err != nil {
			panic(err)
		}
	}()

	ctx, cancel = context.WithTimeout(context.Background(), 2*time.Second)
	defer cancel()
	pErr := client.Ping(ctx, readpref.Primary())

	if pErr != nil {
		return ""
	}

	// get version from admin DB
	logger.Debug("getting mongodb version")
	var commandResult bson.M
	command := bson.D{{"buildInfo", 1}}
	vErr := client.Database("admin").RunCommand(ctx, command).Decode(&commandResult)

	if vErr != nil {
		return ""
	}

	// parse version field to get version information
	ver := fmt.Sprintf("%+v", commandResult["version"])
	return ver
}

func (w MongoDBConnection) GetStatus(logger *cli.Logger) ConnectionStatus {
	return ConnectionStatus{
		Status:  "pass",
		Version: w.GetVersion(logger),
	}
}

func (w MongoDBConnection) WriteScriptTelemetryV1(logrec *ScriptTelemetryRecordV1, logger *cli.Logger) (*Result, error) {
	return commitMongo(w.Config.ConnString, w.Config.ScriptTarget, logrec, logger)
}

func (w MongoDBConnection) WriteScriptTelemetryV2(logrec *ScriptTelemetryRecordV2, logger *cli.Logger) (*Result, error) {
	return commitMongo(w.Config.ConnString, w.Config.ScriptTarget, logrec, logger)
}

func (w MongoDBConnection) WriteEventTelemetryV2(logrec *EventTelemetryRecordV2, logger *cli.Logger) (*Result, error) {
	return commitMongo(w.Config.ConnString, w.Config.EventTarget, logrec, logger)
}

func commitMongo(connStr string, targetCollection string, logrec interface{}, logger *cli.Logger) (*Result, error) {
	// parse and grab database name from uri
	logger.Debug("check connection string")
	connStringInfo, err := connstring.ParseAndValidate(connStr)

	if err != nil {
		return nil, err
	}

	ctx, cancel := context.WithTimeout(context.Background(), 10*time.Second)
	defer cancel()

	logger.Debug("opening mongodb session using connection string")
	client, err := mongo.Connect(ctx, options.Client().ApplyURI(connStr))

	if err != nil {
		return nil, err
	}

	logger.Trace(client)

	logger.Debug("getting target collection")
	// db := session.DB(dialinfo.Database)
	db := client.Database(connStringInfo.Database)
	// c := db.C(targetCollection)
	c := db.Collection(targetCollection)
	logger.Trace(c)

	logger.Debug("opening bulk operation")
	// bulkop := c.Bulk()

	// build sql data info
	logger.Debug("building documents")

	iCtx, cancel := context.WithTimeout(context.Background(), 2*time.Second)
	defer cancel()

	logger.Debug("inserting new document")
	_, txnErr := c.InsertOne(iCtx, logrec)

	if txnErr != nil {
		return nil, txnErr
	}

	// compact collection if requested
	logger.Debug("preparing report")
	return &Result{
		Message: "successfully inserted usage document",
	}, nil
}

// Read methods for MongoDB
func (w MongoDBConnection) ReadScriptTelemetryV1(limit int, offset int, logger *cli.Logger) ([]ScriptTelemetryRecordV1, error) {
	return readMongo[ScriptTelemetryRecordV1](w.Config.ConnString, w.Config.ScriptTarget, nil, limit, offset, logger)
}

func (w MongoDBConnection) ReadScriptTelemetryV2(limit int, offset int, logger *cli.Logger) ([]ScriptTelemetryRecordV2, error) {
	return readMongo[ScriptTelemetryRecordV2](w.Config.ConnString, w.Config.ScriptTarget, nil, limit, offset, logger)
}

func (w MongoDBConnection) ReadEventTelemetryV2(limit int, offset int, logger *cli.Logger) ([]EventTelemetryRecordV2, error) {
	return readMongo[EventTelemetryRecordV2](w.Config.ConnString, w.Config.EventTarget, nil, limit, offset, logger)
}

// Search methods for MongoDB
func (w MongoDBConnection) SearchScriptTelemetryV1(query map[string]interface{}, limit int, offset int, logger *cli.Logger) ([]ScriptTelemetryRecordV1, error) {
	return readMongo[ScriptTelemetryRecordV1](w.Config.ConnString, w.Config.ScriptTarget, query, limit, offset, logger)
}

func (w MongoDBConnection) SearchScriptTelemetryV2(query map[string]interface{}, limit int, offset int, logger *cli.Logger) ([]ScriptTelemetryRecordV2, error) {
	return readMongo[ScriptTelemetryRecordV2](w.Config.ConnString, w.Config.ScriptTarget, query, limit, offset, logger)
}

func (w MongoDBConnection) SearchEventTelemetryV2(query map[string]interface{}, limit int, offset int, logger *cli.Logger) ([]EventTelemetryRecordV2, error) {
	return readMongo[EventTelemetryRecordV2](w.Config.ConnString, w.Config.EventTarget, query, limit, offset, logger)
}

// Generic read function for MongoDB
func readMongo[T any](connStr string, targetCollection string, query map[string]interface{}, limit int, offset int, logger *cli.Logger) ([]T, error) {
	// Parse connection string
	connStringInfo, err := connstring.ParseAndValidate(connStr)
	if err != nil {
		return nil, err
	}

	// Connect to MongoDB
	ctx, cancel := context.WithTimeout(context.Background(), 10*time.Second)
	defer cancel()

	client, err := mongo.Connect(ctx, options.Client().ApplyURI(connStr))
	if err != nil {
		return nil, err
	}
	defer client.Disconnect(ctx)

	// Get collection
	db := client.Database(connStringInfo.Database)
	c := db.Collection(targetCollection)

	// Prepare query options
	findOptions := options.Find()
	findOptions.SetLimit(int64(limit))
	findOptions.SetSkip(int64(offset))
	findOptions.SetSort(bson.D{{"timestamp", -1}}) // Sort by timestamp descending

	// Convert query map to bson.M
	var filter bson.M
	if query != nil {
		filter = bson.M(query)
	} else {
		filter = bson.M{}
	}

	// Execute query
	cursor, err := c.Find(ctx, filter, findOptions)
	if err != nil {
		return nil, err
	}
	defer cursor.Close(ctx)

	// Decode results
	var results []T
	if err = cursor.All(ctx, &results); err != nil {
		return nil, err
	}

	return results, nil
}
