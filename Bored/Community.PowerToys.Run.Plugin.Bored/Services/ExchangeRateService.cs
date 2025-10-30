using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Community.PowerToys.Run.Plugin.Bored.Models;

namespace Community.PowerToys.Run.Plugin.Bored.Services
{
    public class ExchangeRateService : HttpClientService
    {
        private readonly CacheService _cache;
        private const string ApiUrl = "https://open.er-api.com/v6/latest";

        public ExchangeRateService(CacheService cache)
        {
            _cache = cache;
        }

        public async Task<(decimal rate, string date)?> GetExchangeRateAsync(string from, string to, decimal amount = 1, CancellationToken cancellationToken = default)
        {
            // Cache exchange rates for 5 minutes
            var cacheKey = $"exchange_{from}";
            var response = await _cache.GetOrAddAsync(cacheKey, async () =>
            {
                var url = $"{ApiUrl}/{from}";
                return await GetJsonAsync<OpenExchangeRateResponse>(url, cancellationToken);
            }, TimeSpan.FromMinutes(5));

            if (response?.Rates != null && response.Rates.ContainsKey(to))
            {
                var rate = response.Rates[to];
                var convertedAmount = amount * rate;
                return (convertedAmount, response.TimeLastUpdate ?? "Unknown");
            }

            return null;
        }

        public bool ValidateCurrencyCode(string code)
        {
            return !string.IsNullOrWhiteSpace(code) && code.Length == 3 && code.ToUpper() == code.ToUpper();
        }
    }
}
