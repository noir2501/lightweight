using System;
using System.Collections;
using System.Web;
using System.Web.Caching;

namespace Lightweight.Business.Providers.Caching
{
    public class WebCacheProvider : CacheProviderBase
    {

        private readonly Cache _cache;

        public WebCacheProvider()
        {
            _cache = HttpRuntime.Cache;
        }

        public WebCacheProvider(Cache cache)
        {
            _cache = cache;
        }

        public override int Count
        {
            get { return _cache.Count; }
        }

        public override long EffectivePercentagePhysicalMemoryLimit
        {
            get { return _cache.EffectivePercentagePhysicalMemoryLimit; }
        }

        public override long EffectivePrivateBytesLimit
        {
            get { return _cache.EffectivePrivateBytesLimit; }
        }

        public override object this[string key]
        {
            get { return _cache[key]; }
            set { _cache[key] = value; }
        }

        public override object Add(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration,
                                   TimeSpan slidingExpiration, CacheItemPriority priority,
                                   CacheItemRemovedCallback onRemoveCallback)
        {
            return _cache.Add(key, value, dependencies, absoluteExpiration, slidingExpiration, priority,
                              onRemoveCallback);
        }

        public override object Get(string key)
        {
            return _cache.Get(key);
        }

        public override IDictionaryEnumerator GetEnumerator()
        {
            return _cache.GetEnumerator();
        }

        public override void Insert(string key, object value)
        {
            _cache.Insert(key, value);
        }

        public override void Insert(string key, object value, CacheDependency dependencies)
        {
            _cache.Insert(key, value, dependencies);
        }

        public override void Insert(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration,
                                    TimeSpan slidingExpiration)
        {
            _cache.Insert(key, value, dependencies, absoluteExpiration, slidingExpiration);
        }

        public override void Insert(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration,
                                    TimeSpan slidingExpiration, CacheItemPriority priority,
                                    CacheItemRemovedCallback onRemoveCallback)
        {
            _cache.Insert(key, value, dependencies, absoluteExpiration, slidingExpiration, priority, onRemoveCallback);
        }

        public override object Remove(string key)
        {
            return _cache.Remove(key);
        }
    }
}