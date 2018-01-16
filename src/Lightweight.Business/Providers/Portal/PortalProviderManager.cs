using System;
using System.Configuration;
using System.Web.Configuration;

namespace Lightweight.Business.Providers.Portal
{
    public class PortalProviderManager
    {
        private static PortalProviderBase defaultProvider;
        private static PortalProviderCollection providers;

        static PortalProviderManager()
        {
            Initialize();
        }

        private static void Initialize()
        {
            ProviderConfiguration configuration =
                (ProviderConfiguration) ConfigurationManager.GetSection("portal-providers");

            if (configuration == null)
                throw new ConfigurationErrorsException
                    ("The portal-providers configuration section is not set correctly.");

            providers = new PortalProviderCollection();

            ProvidersHelper.InstantiateProviders(configuration.Providers
                                                 , providers, typeof (PortalProviderBase));

            providers.SetReadOnly();

            defaultProvider = providers[configuration.Default];

            if (defaultProvider == null)
                throw new Exception("No default settings provider is defined for the portal-providers section.");
        }

        public static PortalProviderBase Provider
        {
            get { return defaultProvider; }
        }

        public static PortalProviderCollection Providers
        {
            get { return providers; }
        }
    }
}