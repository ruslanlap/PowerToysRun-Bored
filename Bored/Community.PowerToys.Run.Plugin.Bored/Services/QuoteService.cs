using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Community.PowerToys.Run.Plugin.Bored.Models;

namespace Community.PowerToys.Run.Plugin.Bored.Services
{
    public class QuoteService : HttpClientService
    {
        private readonly CacheService _cache;
        private const string ApiUrl = "https://zenquotes.io/api/random";

        public QuoteService(CacheService cache)
        {
            _cache = cache;
        }

        public async Task<QuoteData?> GetRandomQuoteAsync(CancellationToken cancellationToken = default)
        {
            // Don't cache - we want fresh quotes every time
            var response = await GetJsonAsync<ZenQuoteResponse[]>(ApiUrl, cancellationToken);
            var quote = response?.FirstOrDefault();
            
            if (quote != null)
            {
                return new QuoteData
                {
                    QuoteText = quote.Q,
                    QuoteAuthor = quote.A
                };
            }
            
            return null;
        }
    }
}
