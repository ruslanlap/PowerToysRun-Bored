using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Community.PowerToys.Run.Plugin.Bored.Models
{
    public class DogResponse
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    public class DogListResponse
    {
        [JsonPropertyName("message")]
        public Dictionary<string, List<string>>? Message { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }
}
