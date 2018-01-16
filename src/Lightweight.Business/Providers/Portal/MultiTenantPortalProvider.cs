using System;
using System.Configuration.Provider;
using System.Web;
using Lightweight.Business.Providers.Caching;
using Lightweight.Business.Providers.NHibernateSession;
using Lightweight.Business.Repository.Entities;

namespace Lightweight.Business.Providers.Portal
{
    public class MultiTenantPortalProvider : PortalProviderBase
    {

        private string _applicationName;
        public string ApplicationName
        {
            get
            {
                if (!string.IsNullOrEmpty(_applicationName))
                    return _applicationName;

                if (HttpContext.Current != null)
                {
                    // get the request host
                    string host = HttpContext.Current.Request.Headers["Host"]
                        .Split(':')[0] // removes port from host
                        .Replace("www.", string.Empty); // removes www. from host

                    return host;
                }

                throw new ArgumentNullException("ApplicationName", "Could not determine the application name for the portal provider.");
            }
            set { _applicationName = value; }
        }

        private PortalRepository _portalRepository
        {
            get
            {
                return new PortalRepository(NHibernateSessionProvider.Instance.CurrentSession);
            }
        }

        private bool _caching = false;

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentException("config");

            if (string.IsNullOrEmpty(name))
                name = "MultiTenantPortalProvider";

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Multi-tenant navigation provider");
            }

            base.Initialize(name, config);

            string applicationName = config["applicationName"];

            if (!string.IsNullOrEmpty(applicationName))
                ApplicationName = applicationName;

            this._caching = GetConfigValue(config["caching"], false);
        }

        #region Provider helpers

        //
        // Helper functions to retrieve config values from the configuration file.
        //

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }

        private int GetConfigValue(string configValue, int defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return
                Convert.ToInt32(configValue);
        }

        private bool GetConfigValue(string configValue, bool defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return
                Convert.ToBoolean(configValue);
        }

        #endregion

        public override Model.Entities.Portal GetCurrentPortal()
        {
            Model.Entities.Portal portal = null;
            CacheProviderBase cacheProvider = null;

            string cacheKey = string.Format("PORTAL:{0}", ApplicationName); // compose the cache key = constant + host

            if (this._caching)
            {
                cacheProvider = CacheProviderManager.Provider;
                portal = cacheProvider.Get(cacheKey) as Model.Entities.Portal;
            }

            if (portal == null) // if not in cache, request the portal from database
            {
                portal = _portalRepository.GetPortalByUrl(ApplicationName) ?? _portalRepository.GetPortalByAliasUrl(ApplicationName);

                //if portal url was not found in database, look in portal aliases

                if (this._caching && portal != null) // portal was found, cache it
                    cacheProvider.Insert(cacheKey, portal);
            }

            // this will raise a configuration error if a portal is not defined for the current host

            //if (_portal == null)
            //    throw new ProviderException(string.Format("Could not find portal for host '{0}'.", ApplicationName));
            return portal;
        }
    }
}
