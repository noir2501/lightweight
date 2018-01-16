using System;
using System.Configuration;
using System.Web.Configuration;

namespace Lightweight.Business.Providers.Caching
{
    public class CacheProviderManager
    {
        private static CacheProviderBase defaultProvider;
        private static CacheProviderCollection providers;

        static CacheProviderManager()
        {
            Initialize();
        }

        private static void Initialize()
        {
            ProviderConfiguration configuration = (ProviderConfiguration)ConfigurationManager.GetSection("cache-providers");

            if (configuration == null)
                throw new ConfigurationErrorsException
                    ("The cache-providers configuration section is not set correctly.");

            providers = new CacheProviderCollection();

            ProvidersHelper.InstantiateProviders(configuration.Providers
                , providers, typeof(CacheProviderBase));

            providers.SetReadOnly();

            defaultProvider = providers[configuration.Default];

            if (defaultProvider == null)
                throw new Exception("No default cache provider is defined for the cache-providers section.");
        }

        public static CacheProviderBase Provider
        {
            get
            {
                return defaultProvider;
            }
        }

        public static CacheProviderCollection Providers
        {
            get
            {
                return providers;
            }
        }
    }
}
