using System.Threading;
using System.Threading.Tasks;
using Community.PowerToys.Run.Plugin.Bored.Models;

namespace Community.PowerToys.Run.Plugin.Bored.Services
{
    public class DogService : HttpClientService
    {
        private readonly CacheService _cache;
        private const string ApiUrl = "https://dog.ceo/api/breeds/image/random";

        public DogService(CacheService cache)
        {
            _cache = cache;
        }

        public async Task<DogResponse?> GetRandomDogImageAsync(CancellationToken cancellationToken = default)
        {
            // Don't cache - we want fresh dog images every time
            return await GetJsonAsync<DogResponse>(ApiUrl, cancellationToken);
        }
    }
}
