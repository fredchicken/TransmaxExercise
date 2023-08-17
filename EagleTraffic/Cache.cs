using Microsoft.Extensions.Caching.Distributed;
using EagleTraffic.Models;
using Newtonsoft.Json;

namespace EagleTraffic
{
    public class Cache
    {
        private readonly IDistributedCache _cache;

        private readonly DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(2))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1));
        public Cache(IDistributedCache cache)
        {
            this._cache = cache;
        }

        public async Task<T> Get<T>(string key) where T: class
        {
            var cachedResponse = await _cache.GetStringAsync(key);
            return (cachedResponse == null) ? null : 
                JsonConvert.DeserializeObject<T>(cachedResponse,new JsonSerializerSettings() { 
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
        }

        public async Task Set<T>(string key, T value) where T : class
        {
            var response = JsonConvert.SerializeObject(value, new JsonSerializerSettings() {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            await _cache.SetStringAsync(key, response, options);
        }

        public async Task Clear(string key) 
        {
            await _cache.RemoveAsync(key);
        }
    }
}
