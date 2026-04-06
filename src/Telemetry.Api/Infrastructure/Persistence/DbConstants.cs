namespace Telemetry.Api.Infrastructure.Persistence
{
    internal static class DbConstants
    {
        public static class Tables
        {
            public const string Events = "events";
            public const string Scripts = "scripts";
        }

        public static class Columns
        {
            public const string Id = "_id";
            public const string SessionId = "sessionid";
            public const string Timestamp = "timestamp";
            public const string Username = "username";
            public const string HostUsername = "host_user";
            public const string RevitBuild = "revitbuild";
            public const string RevitVersion = "revit";
            public const string PyRevitVersion = "pyrevit";
            public const string CloneName = "clone";
            public const string IsDebug = "debug";
            public const string IsConfig = "config";
            public const string IsExecFromGui = "from_gui";
            public const string ExecId = "exec_id";
            public const string ExecTimestamp = "exec_timestamp";
            public const string CommandBundle = "commandbundle";
            public const string CommandExtension = "commandextension";
            public const string CommandName = "commandname";
            public const string CommandUniqueName = "commanduniquename";
            public const string DocumentName = "docname";
            public const string DocumentPath = "docpath";
            public const string ResultCode = "resultcode";
            public const string ScriptPath = "scriptpath";
            public const string CommandResults = "commandresults";

            public const string TraceMessage = "trace_message";
            public const string EngineType = "engine_type";
            public const string EngineVersion = "engine_configs";
            public const string EngineConfigs = "configs";
            public const string EngineSysPaths = "engine_syspath";

            public const string HandlerId = "handler_id";
            public const string EventType = "type";
            public const string Status = "status";
            public const string Cancelled = "cancelled";
            public const string Cancellable = "cancellable";
            public const string DocumentId = "docid";
            public const string DocumentType = "doctype";
            public const string DocumentTemplate = "doctemplate";
            public const string ProjectName = "projectname";
            public const string ProjectNum = "projectnum";
            public const string EventArgs = "args";
        }
    }
}