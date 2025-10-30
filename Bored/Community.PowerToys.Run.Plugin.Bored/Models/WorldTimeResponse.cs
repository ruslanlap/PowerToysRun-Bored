using System.Text.Json.Serialization;

namespace Community.PowerToys.Run.Plugin.Bored.Models
{
    public class WorldTimeResponse
    {
        [JsonPropertyName("datetime")]
        public string? DateTime { get; set; }

        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }

        [JsonPropertyName("utc_datetime")]
        public string? UtcDateTime { get; set; }

        [JsonPropertyName("utc_offset")]
        public string? UtcOffset { get; set; }

        [JsonPropertyName("day_of_week")]
        public int DayOfWeek { get; set; }

        [JsonPropertyName("day_of_year")]
        public int DayOfYear { get; set; }

        [JsonPropertyName("week_number")]
        public int WeekNumber { get; set; }
    }
}
