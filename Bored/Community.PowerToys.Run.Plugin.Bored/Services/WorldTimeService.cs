using System;
using System.Threading;
using System.Threading.Tasks;
using Community.PowerToys.Run.Plugin.Bored.Models;

namespace Community.PowerToys.Run.Plugin.Bored.Services
{
    public class WorldTimeService : HttpClientService
    {
        private readonly CacheService _cache;
        private const string ApiUrl = "https://worldtimeapi.org/api/timezone";

        public WorldTimeService(CacheService cache)
        {
            _cache = cache;
        }

        public async Task<WorldTimeResponse?> GetTimeAsync(string location, CancellationToken cancellationToken = default)
        {
            var normalizedLocation = location.Replace(" ", "_");
            // Cache for 30 seconds only - time changes
            var cacheKey = $"time_{normalizedLocation}_{DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond / 30}";
            var url = $"{ApiUrl}/{normalizedLocation}";
            return await GetJsonAsync<WorldTimeResponse>(url, cancellationToken);
        }
    }
}
