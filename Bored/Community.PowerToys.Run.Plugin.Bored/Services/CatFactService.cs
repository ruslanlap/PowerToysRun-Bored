using System.Threading;
using System.Threading.Tasks;
using Community.PowerToys.Run.Plugin.Bored.Models;

namespace Community.PowerToys.Run.Plugin.Bored.Services
{
    public class CatFactService : HttpClientService
    {
        private readonly CacheService _cache;
        private const string ApiUrl = "https://catfact.ninja/fact";

        public CatFactService(CacheService cache)
        {
            _cache = cache;
        }

        public async Task<CatFactResponse?> GetRandomFactAsync(CancellationToken cancellationToken = default)
        {
            // Don't cache - we want fresh facts every time
            return await GetJsonAsync<CatFactResponse>(ApiUrl, cancellationToken);
        }
    }
}
