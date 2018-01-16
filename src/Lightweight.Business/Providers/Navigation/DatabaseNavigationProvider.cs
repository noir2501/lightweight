using System.Collections.Generic;
using System.Web;
using Lightweight.Business.Exceptions;
using System;
using Lightweight.Business.Providers.NHibernateSession;
using Lightweight.Business.Providers.Portal;
using Lightweight.Business.Repository.Entities;

namespace Lightweight.Business.Providers.Navigation
{
    public class DatabaseNavigationProvider : NavigationProviderBase
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

                throw new ArgumentNullException("ApplicationName", "Could not determine the application name for the navigation provider.");
            }
            set { _applicationName = value; }
        }

        private PageRepository NavigationRepository
        {
            get { return new PageRepository(NHibernateSessionProvider.Instance.CurrentSession); }
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentException("config");

            if (string.IsNullOrEmpty(name))
                name = "DatabaseNavigationProvider";

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Database navigation provider");
            }

            base.Initialize(name, config);

            string applicationName = config["applicationName"];

            if (!string.IsNullOrEmpty(applicationName))
                ApplicationName = applicationName;
        }

        public override List<Model.Entities.Page> GetRoleNavigation(string role)
        {
            return NavigationRepository.GetRoleNavigation(role, PortalProviderManager.Provider.GetCurrentPortal().Tenant.Id);
        }

        public override List<Model.Entities.Page> GetUserNavigation(string username)
        {
            return NavigationRepository.GetUserNavigations(username, PortalProviderManager.Provider.GetCurrentPortal().Tenant.Id);
        }

        public override string GeneratePageSlug(string title, string parentSlug, bool unique)
        {
            string slug = title.Replace(' ', '-');

            return slug;
        }
    }
}