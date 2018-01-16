using System;
using System.Collections.Generic;
using System.Configuration.Provider;

namespace Lightweight.Business.Providers.Logging
{
    public abstract class LoggingProviderBase : ProviderBase
    {
        public abstract string LogEntry(ILogEntry entry);
        public abstract string LogEntry(string user, string source, string type, int code, string message);
        public abstract string LogEntry(string user, string source, string type, int code, string message, Exception exception);

        public abstract IEnumerable<ILogEntry> GetLogs(string logger);
        public abstract IEnumerable<ILogEntry> GetUserLogs(string logger, string user);
        public abstract IEnumerable<ILogEntry> GetSourceLogs(string logger, string source);
        public abstract IEnumerable<ILogEntry> GetUserSourceLogs(string logger, string user, string source);
    }
}