using Lightweight.Model.Entities;
using System.Collections.Generic;

namespace Lightweight.Business.Providers.Navigation
{
    public abstract class NavigationProviderBase : System.Configuration.Provider.ProviderBase
    {
        public abstract List<Page> GetRoleNavigation(string role);
        public abstract List<Page> GetUserNavigation(string username);
        public abstract string GeneratePageSlug(string title, string parentSlug, bool unique);
    }
}