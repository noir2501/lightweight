using System;
using System.Configuration;
using System.Web.Configuration;

namespace Lightweight.Business.Providers.Logging
{
    public class LoggingProviderManager
    {
        private static LoggingProviderBase defaultProvider;
        private static LoggingProviderCollection providers;

        static LoggingProviderManager()
        {
            Initialize();
        }

        private static void Initialize()
        {
            ProviderConfiguration configuration = (ProviderConfiguration)ConfigurationManager.GetSection("logging-providers");

            if (configuration == null)
                throw new ConfigurationErrorsException
                    ("The logging-providers configuration section is not set correctly.");

            providers = new LoggingProviderCollection();

            ProvidersHelper.InstantiateProviders(configuration.Providers
                , providers, typeof(LoggingProviderBase));

            providers.SetReadOnly();

            defaultProvider = providers[configuration.Default];

            if (defaultProvider == null)
                throw new Exception("No default logging provider is defined for the logging-providers section.");
        }

        public static LoggingProviderBase Provider
        {
            get
            {
                return defaultProvider;
            }
        }

        public static LoggingProviderCollection Providers
        {
            get
            {
                return providers;
            }
        }
    }
}
