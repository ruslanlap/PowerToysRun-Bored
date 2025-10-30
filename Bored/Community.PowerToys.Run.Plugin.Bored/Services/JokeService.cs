using System.Threading;
using System.Threading.Tasks;
using Community.PowerToys.Run.Plugin.Bored.Models;

namespace Community.PowerToys.Run.Plugin.Bored.Services
{
    public class JokeService : HttpClientService
    {
        private readonly CacheService _cache;
        private const string ApiUrl = "https://official-joke-api.appspot.com/random_joke";

        public JokeService(CacheService cache)
        {
            _cache = cache;
        }

        public async Task<JokeResponse?> GetRandomJokeAsync(CancellationToken cancellationToken = default)
        {
            // Don't cache - we want fresh jokes every time
            return await GetJsonAsync<JokeResponse>(ApiUrl, cancellationToken);
        }
    }
}
