namespace Lightweight.Business.Providers.Portal
{
    public abstract class PortalProviderBase : System.Configuration.Provider.ProviderBase
    {
        public abstract Model.Entities.Portal GetCurrentPortal();
    }
}