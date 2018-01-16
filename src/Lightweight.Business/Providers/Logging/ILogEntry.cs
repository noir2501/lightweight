using System;

namespace Lightweight.Business.Providers.Logging
{
    public interface ILogEntry
    {
        Guid ID { get; set; }
        string Message { get; set; }

        string Logger { get; set; }
        string User { get; set; }
        string Source { get; set; }
        string Type { get; set; }
        int Code { get; set; }

        string Host { get; set; }
        string Level { get; set; }
        DateTime TimeUtc { get; set; }

        string ErrorXml { get; set; }
    }
}