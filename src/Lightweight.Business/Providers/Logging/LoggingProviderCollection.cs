using System.Configuration.Provider;

namespace Lightweight.Business.Providers.Logging
{
    public class LoggingProviderCollection : ProviderCollection
    {
        new public LoggingProviderBase this[string name]
        {
            get { return (LoggingProviderBase)base[name]; }
        }
    }
}