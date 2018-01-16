using System;
using System.Collections;
using System.Configuration.Provider;
using System.Web.Caching;

namespace Lightweight.Business.Providers.Caching
{
    public abstract class CacheProviderBase : ProviderBase, IEnumerable
    {
        public abstract int Count { get; }

        public abstract long EffectivePercentagePhysicalMemoryLimit { get; }
        public abstract long EffectivePrivateBytesLimit { get; }

        public abstract object Add(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback);

        public abstract object Get(string key);

        public abstract void Insert(string key, object value);
        public abstract void Insert(string key, object value, CacheDependency dependencies);
        public abstract void Insert(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration);
        public abstract void Insert(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback);

        public abstract object Remove(string key);

        public abstract object this[string key] { get; set; }
        public abstract IDictionaryEnumerator GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
