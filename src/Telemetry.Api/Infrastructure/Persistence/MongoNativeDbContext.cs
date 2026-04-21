using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Telemetry.Api.Application.Interfaces;
using Telemetry.Api.Domain.Constants;
using Telemetry.Api.Domain.Models;
using Telemetry.Api.JsonConverters;

namespace Telemetry.Api.Infrastructure.Persistence
{
    internal sealed class MongoNativeDbContext : IApplicationDbContext
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoDatabase _mongoDatabase;

        private readonly IMongoCollection<EventRecord> _eventsCollection;
        private readonly IMongoCollection<ScriptRecord> _scriptCollection;

        static MongoNativeDbContext()
        {
            OnModelCreating();
        }

        public MongoNativeDbContext(IMongoClient mongoClient, string mongodbName)
        {
            _mongoClient = mongoClient;
            _mongoDatabase = _mongoClient.GetDatabase(mongodbName);
            
            _eventsCollection = _mongoDatabase.GetCollection<EventRecord>(GetCollectionName<EventRecord>());
            _scriptCollection = _mongoDatabase.GetCollection<ScriptRecord>(GetCollectionName<ScriptRecord>());
        }

        public async Task AddEventRecord(EventRecord eventRecord, CancellationToken cancellationToken)
        {
            await _eventsCollection.InsertOneAsync(eventRecord, new InsertOneOptions(), cancellationToken);
        }

        public async Task AddScriptRecord(ScriptRecord scriptRecord, CancellationToken cancellationToken)
        {
            await _scriptCollection.InsertOneAsync(scriptRecord, new InsertOneOptions(), cancellationToken);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public async Task<bool> CanConnectAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _mongoClient.GetDatabase("admin")
                    .RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1), cancellationToken: cancellationToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetDbProvider()
        {
            return "MongoDB Native";
        }

        public async Task<string> GetDbVersionAsync(CancellationToken cancellationToken)
        {
            BsonDocumentCommand<BsonDocument> command = new(new BsonDocument {{"buildInfo", 1}});
            BsonValue? version = await _mongoDatabase.RunCommandAsync(command, cancellationToken: cancellationToken);
            return version["version"].AsString;
        }

        private static string GetCollectionName<T>()
        {
            var table = typeof(T).GetCustomAttribute<TableAttribute>();
            return table?.Name ?? typeof(T).Name;
        }

        private static void OnModelCreating()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(EventRecord)))
            {
                RegisterClassEventRecord();
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(ScriptRecord)))
            {
                RegisterClassScriptRecord();
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(TraceInfo)))
            {                BsonClassMap.RegisterClassMap<TraceInfo>(classMap =>
                {
                    classMap.AutoMap();
                    classMap.MapProperty(p => p.Message).SetElementName(PropertyNames.TraceMessage);
                    classMap.MapProperty(p => p.Engine).SetElementName(PropertyNames.Engine);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(EngineInfo)))
            {
                BsonClassMap.RegisterClassMap<EngineInfo>(classMap =>
                {
                    classMap.AutoMap();
                    classMap.MapProperty(p => p.Type).SetElementName(PropertyNames.EngineType);
                    classMap.MapProperty(p => p.Version).SetElementName(PropertyNames.EngineVersion);
                    classMap.MapProperty(p => p.SysPaths).SetElementName(PropertyNames.EngineSysPaths);

                    classMap.MapProperty(p => p.Configs)
                        .SetElementName(PropertyNames.EngineConfigs)
                        .SetSerializer(new DynamicDataBsonSerializer());
                });
            }
        }

        private static void RegisterClassEventRecord()
        {
            BsonClassMap.RegisterClassMap<EventRecord>(classMap =>
            {
                classMap.AutoMap();

                classMap.MapIdProperty(p => p.Id)
                    .SetElementName(PropertyNames.Id)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));

                classMap.MapProperty(p => p.HandlerId)
                    .SetElementName(PropertyNames.HandlerId)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));

                classMap.MapProperty(p => p.EventType).SetElementName(PropertyNames.EventType);
                classMap.MapProperty(p => p.Status).SetElementName(PropertyNames.Status);

                classMap.MapProperty(p => p.Timestamp)
                    .SetElementName(PropertyNames.Timestamp)
                    .SetSerializer(new DateTimeOffsetSerializer(BsonType.DateTime));

                classMap.MapProperty(p => p.Username).SetElementName(PropertyNames.Username);
                classMap.MapProperty(p => p.HostUsername).SetElementName(PropertyNames.HostUsername);
                classMap.MapProperty(p => p.RevitBuild).SetElementName(PropertyNames.RevitBuild);
                classMap.MapProperty(p => p.RevitVersion).SetElementName(PropertyNames.RevitVersion);
                classMap.MapProperty(p => p.Cancelled).SetElementName(PropertyNames.Cancelled);
                classMap.MapProperty(p => p.Cancellable).SetElementName(PropertyNames.Cancellable);
                classMap.MapProperty(p => p.DocumentId).SetElementName(PropertyNames.DocumentId);
                classMap.MapProperty(p => p.DocumentType).SetElementName(PropertyNames.DocumentType);
                classMap.MapProperty(p => p.DocumentTemplate).SetElementName(PropertyNames.DocumentTemplate);
                classMap.MapProperty(p => p.DocumentName).SetElementName(PropertyNames.DocumentName);
                classMap.MapProperty(p => p.DocumentPath).SetElementName(PropertyNames.DocumentPath);
                classMap.MapProperty(p => p.ProjectName).SetElementName(PropertyNames.ProjectName);
                classMap.MapProperty(p => p.ProjectNum).SetElementName(PropertyNames.ProjectNum);

                classMap.MapProperty(p => p.EventArgs)
                    .SetElementName(PropertyNames.EventArgs)
                    .SetSerializer(new DynamicDataBsonSerializer());
            });
        }

        private static void RegisterClassScriptRecord()
        {
            BsonClassMap.RegisterClassMap<ScriptRecord>(classMap =>
            {
                classMap.AutoMap();

                classMap.MapIdProperty(p => p.Id)
                    .SetElementName(PropertyNames.Id)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));

                classMap.MapProperty(p => p.SessionId)
                    .SetElementName(PropertyNames.SessionId)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));

                classMap.MapProperty(p => p.Timestamp)
                    .SetElementName(PropertyNames.Timestamp)
                    .SetSerializer(new DateTimeOffsetSerializer(BsonType.DateTime));

                classMap.MapProperty(p => p.Username).SetElementName(PropertyNames.Username);
                classMap.MapProperty(p => p.HostUsername).SetElementName(PropertyNames.HostUsername);
                classMap.MapProperty(p => p.RevitBuild).SetElementName(PropertyNames.RevitBuild);
                classMap.MapProperty(p => p.RevitVersion).SetElementName(PropertyNames.RevitVersion);
                classMap.MapProperty(p => p.PyRevitVersion).SetElementName(PropertyNames.PyRevitVersion);
                classMap.MapProperty(p => p.CloneName).SetElementName(PropertyNames.CloneName);
                classMap.MapProperty(p => p.IsDebug).SetElementName(PropertyNames.IsDebug);
                classMap.MapProperty(p => p.IsConfig).SetElementName(PropertyNames.IsConfig);
                classMap.MapProperty(p => p.IsExecFromGui).SetElementName(PropertyNames.IsExecFromGui);
                classMap.MapProperty(p => p.ExecId).SetElementName(PropertyNames.ExecId);

                classMap.MapProperty(p => p.ExecTimestamp)
                    .SetElementName(PropertyNames.ExecTimestamp)
                    .SetSerializer(new DateTimeOffsetSerializer(BsonType.DateTime));

                classMap.MapProperty(p => p.CommandBundle).SetElementName(PropertyNames.CommandBundle);
                classMap.MapProperty(p => p.CommandExtension).SetElementName(PropertyNames.CommandExtension);
                classMap.MapProperty(p => p.CommandName).SetElementName(PropertyNames.CommandName);
                classMap.MapProperty(p => p.CommandUniqueName).SetElementName(PropertyNames.CommandUniqueName);
                classMap.MapProperty(p => p.DocumentName).SetElementName(PropertyNames.DocumentName);
                classMap.MapProperty(p => p.DocumentPath).SetElementName(PropertyNames.DocumentPath);
                classMap.MapProperty(p => p.ResultCode).SetElementName(PropertyNames.ResultCode);
                classMap.MapProperty(p => p.ScriptPath).SetElementName(PropertyNames.ScriptPath);
                classMap.MapProperty(p => p.Trace).SetElementName(PropertyNames.Trace);

                classMap.MapProperty(p => p.CommandResults)
                    .SetElementName(PropertyNames.CommandResults)
                    .SetSerializer(new DynamicDataBsonSerializer());
            });
        }
    }
}