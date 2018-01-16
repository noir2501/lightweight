using System;
using System.Configuration;
using System.Configuration.Provider;
using System.Web;
using System.Web.Profile;
using Lightweight.Business.Providers.NHibernateSession;
using Lightweight.Business.Providers.Portal;
using Lightweight.Business.Repository.Entities;
using Lightweight.Business.Helpers;
using Lightweight.Model.Entities;

namespace Lightweight.Business.Providers.Membership
{
    public class MultiTenantProfileProvider : ProfileProvider
    {
        private string _applicationName;
        public override string ApplicationName
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

                throw new ArgumentNullException("ApplicationName", "Could not determine the application name for the membership provider.");
            }
            set { _applicationName = value; }
        }

        public string ConnectionName { get; set; }

        private UserRepository UserRepository
        {
            get { return new UserRepository(NHibernateSessionProvider.Instance.CurrentSession); }
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentException("config");

            if (string.IsNullOrEmpty(name))
                name = "MultiTenantProfileProvider";

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Multi-tenant profile provider");
            }

            base.Initialize(name, config);

            string applicationName = GetConfigValue(config["applicationName"], null);

            if (!string.IsNullOrEmpty(applicationName))
                ApplicationName = applicationName;

            string connectionStringName = GetConfigValue(config["connectionStringName"], string.Empty);
            if (string.IsNullOrEmpty(connectionStringName))
                throw new ProviderException("The connection string name is missing or empty.");

            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connectionStringSettings == null || string.IsNullOrEmpty(connectionStringSettings.ConnectionString))
                throw new ProviderException("The connection string is missing or blank.");

            ConnectionName = connectionStringSettings.ConnectionString;

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


        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            string username = (string)context["UserName"];
            bool isAuthenticated = (bool)context["IsAuthenticated"];

            if (!isAuthenticated)
                throw new ProviderException("Anonymous profile data is not supported by this profile provider.");

            Guid userID; // the user's unique identifier for this profile, if found - not used further
            var profile = UserRepository.GetProfileByUserName(username, PortalProviderManager.Provider.GetCurrentPortal().Tenant.Id, out userID);

            SettingsPropertyValueCollection result = new SettingsPropertyValueCollection();

            foreach (SettingsProperty property in collection)
            {
                SettingsPropertyValue value = new SettingsPropertyValue(property)
                                                  {
                                                      PropertyValue = ReflectionHelper.GetPropertyValue(profile, property.Name, true)
                                                  };

                //TODO: get property from user entity
                result.Add(value);
            }

            return result;
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            string username = (string)context["UserName"];
            bool isAuthenticated = (bool)context["IsAuthenticated"];

            if (!isAuthenticated)
                throw new ProviderException("Anonymous profile data is not supported by this profile provider.");

            Guid userId; // the user's unique identifier for this profile
            var profile = UserRepository.GetProfileByUserName(username, PortalProviderManager.Provider.GetCurrentPortal().Tenant.Id, out userId);

            if (userId == default (Guid))
                throw new ProviderException(string.Format("The specified user does not exist: {0}", username));

            if (profile == null)
                profile = new UserProfile(new User(userId));

            foreach (SettingsPropertyValue property in collection)
                ReflectionHelper.SetPropertyValue(profile, property.Name, property.PropertyValue);

            profile.LastUpdated = DateTime.Now;
            try
            {
                UserRepository.SaveProfile(profile);
            }
            catch (Exception ex)
            {
                throw new ProviderException(string.Format("Failed to update the user profile for user '{0}'.", username), ex);
            }
        }

        public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = 1;
            return new ProfileInfoCollection()
                       {
                           new ProfileInfo(usernameToMatch, false, DateTime.Now, DateTime.Now, 0)
                       };
        }

        public override int DeleteInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            throw new NotImplementedException();
        }

        public override int DeleteProfiles(string[] usernames)
        {
            throw new NotImplementedException();
        }

        public override int DeleteProfiles(System.Web.Profile.ProfileInfoCollection profiles)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection FindInactiveProfilesByUserName(System.Web.Profile.ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection GetAllInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection GetAllProfiles(System.Web.Profile.ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            throw new NotImplementedException();
        }
    }
}
