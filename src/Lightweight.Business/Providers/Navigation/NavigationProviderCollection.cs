using System.Configuration.Provider;

namespace Lightweight.Business.Providers.Navigation
{
    public class NavigationProviderCollection : ProviderCollection
    {
        new public NavigationProviderBase this[string name]
        {
            get { return (NavigationProviderBase)base[name]; }
        }
    }
}