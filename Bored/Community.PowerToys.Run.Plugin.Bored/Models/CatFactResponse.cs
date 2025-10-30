using System.Text.Json.Serialization;

namespace Community.PowerToys.Run.Plugin.Bored.Models
{
    public class CatFactResponse
    {
        [JsonPropertyName("fact")]
        public string? Fact { get; set; }

        [JsonPropertyName("length")]
        public int Length { get; set; }
    }
}
