using System;
using System.Configuration;
using System.Web.Configuration;

namespace Lightweight.Business.Providers.Mail
{
    public class MailServiceProviderManager
    {
        private static MailServiceProviderBase defaultProvider;
        private static MailServiceProviderCollection providers;

        static MailServiceProviderManager()
        {
            Initialize();
        }

        private static void Initialize()
        {
           ProviderConfiguration configuration = (ProviderConfiguration)ConfigurationManager.GetSection("mail-service-providers");

            if (configuration == null)
                throw new ConfigurationErrorsException
                    ("The mail-service-providers configuration section is not set correctly.");

            providers = new MailServiceProviderCollection();

            ProvidersHelper.InstantiateProviders(configuration.Providers
                , providers, typeof(MailServiceProviderBase));

            providers.SetReadOnly();

            defaultProvider = providers[configuration.Default];

            if (defaultProvider == null)
                throw new Exception("No default MailService provider is defined for the MailService-providers section.");
        }

        public static MailServiceProviderBase Provider
        {
            get
            {
                return defaultProvider;
            }
        }

        public static MailServiceProviderCollection Providers
        {
            get
            {
                return providers;
            }
        }
    }
}