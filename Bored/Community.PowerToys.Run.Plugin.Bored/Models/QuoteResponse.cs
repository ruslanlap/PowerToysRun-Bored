using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Community.PowerToys.Run.Plugin.Bored.Models
{
    public class QuoteResponse
    {
        [JsonPropertyName("data")]
        public List<QuoteData>? Data { get; set; }
    }

    public class QuoteData
    {
        [JsonPropertyName("quoteText")]
        public string? QuoteText { get; set; }

        [JsonPropertyName("quoteAuthor")]
        public string? QuoteAuthor { get; set; }

        [JsonPropertyName("quoteGenre")]
        public string? QuoteGenre { get; set; }
    }

    public class ZenQuoteResponse
    {
        [JsonPropertyName("q")]
        public string? Q { get; set; }

        [JsonPropertyName("a")]
        public string? A { get; set; }

        [JsonPropertyName("h")]
        public string? H { get; set; }
    }
}
