using System;
using System.Text.Json.Serialization;

namespace NotKilledByGoogle.Bot.Config
{
    public class BotConfig
    {
        [JsonPropertyName("apiKey")]
        public string ApiKey { get; set; } = "123456789:ABCDEFGHIJKOPQRSTUVWXYZ1234567890a";
        [JsonPropertyName("broadcastId")]
        public long[] BroadcastId { get; set; } = Array.Empty<long>();
        [JsonPropertyName("adminId")]
        public long AdminId { get; set; } = 0L;
        [JsonPropertyName("graveyardJsonLocation")]
        public string GraveyardJsonLocation { get; set; } = "https://raw.githubusercontent.com/codyogden/killedbygoogle/main/graveyard.json";
    }
}
