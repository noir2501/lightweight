using System;
using System.Configuration;
using System.Web.Configuration;

namespace Lightweight.Business.Providers.Navigation
{
    public class NavigationProviderManager
    {
        private static NavigationProviderBase defaultProvider;
        private static NavigationProviderCollection providers;

        static NavigationProviderManager()
        {
            Initialize();
        }

        private static void Initialize()
        {
            ProviderConfiguration configuration = (ProviderConfiguration)ConfigurationManager.GetSection("navigation-providers");

            if (configuration == null)
                throw new ConfigurationErrorsException
                    ("The navigation-providers configuration section is not set correctly.");

            providers = new NavigationProviderCollection();

            ProvidersHelper.InstantiateProviders(configuration.Providers
                , providers, typeof(NavigationProviderBase));

            providers.SetReadOnly();

            defaultProvider = providers[configuration.Default];

            if (defaultProvider == null)
                throw new Exception("No default navigation provider is defined for the navigation-providers section.");
        }

        public static NavigationProviderBase Provider
        {
            get
            {
                return defaultProvider;
            }
        }

        public static NavigationProviderCollection Providers
        {
            get
            {
                return providers;
            }
        }
    }
}
