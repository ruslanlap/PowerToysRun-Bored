using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Community.PowerToys.Run.Plugin.Bored.Services
{
    public class CacheService
    {
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan DefaultCacheDuration = TimeSpan.FromMinutes(7);

        public CacheService()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public T? GetOrAdd<T>(string key, Func<T> factory, TimeSpan? duration = null)
        {
            if (_cache.TryGetValue(key, out T? cached))
            {
                return cached;
            }

            var value = factory();
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = duration ?? DefaultCacheDuration
            };
            _cache.Set(key, value, cacheEntryOptions);
            return value;
        }

        public async Task<T?> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan? duration = null)
        {
            if (_cache.TryGetValue(key, out T? cached))
            {
                return cached;
            }

            var value = await factory();
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = duration ?? DefaultCacheDuration
            };
            _cache.Set(key, value, cacheEntryOptions);
            return value;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void Clear()
        {
            if (_cache is MemoryCache memoryCache)
            {
                memoryCache.Compact(1.0);
            }
        }
    }
}
