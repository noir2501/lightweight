using System.Configuration.Provider;

namespace Lightweight.Business.Providers.Caching
{
    public class CacheProviderCollection : ProviderCollection
    {
        new public CacheProviderBase this[string name]
        {
            get { return (CacheProviderBase)base[name]; }
        }
    }
}
