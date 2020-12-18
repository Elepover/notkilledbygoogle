using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace NotKilledByGoogle.Bot.Grave
{
    /// <summary>
    /// Represents a gravestone in Killed by Google graveyard.
    /// </summary>
    public class Gravestone
    {
        public static readonly JsonSerializerOptions SerializerOptions = new()
        {
            Converters =
            {
                new DateTimeOffsetConverter(),
                new DeceasedTypeConverter()
            }
        };
        
        [JsonPropertyName("dateClose")]
        public DateTimeOffset DateClose { get; set; }
        [JsonPropertyName("dateOpen")]
        public DateTimeOffset DateOpen { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; } = "";
        [JsonPropertyName("link")]
        public string ReferenceLink { get; set; } = "";
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        [JsonPropertyName("type")]
        public DeceasedType DeceasedType { get; set; }

        /// <summary>
        /// Parse a piece of string to a <see cref="Gravestone"/> asynchronously.
        /// </summary>
        /// <param name="raw">Raw string.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to cancel this task.</param>
        /// <inheritdoc cref="JsonSerializer.DeserializeAsync"/>
        public static async Task<Gravestone> ParseAsync(string raw, CancellationToken cancellationToken = default)
        {
            await using var ms = new MemoryStream(Encoding.UTF8.GetBytes(raw));
            return Utils.ThrowIfNull(
                await JsonSerializer.DeserializeAsync<Gravestone>(
                    ms,
                    SerializerOptions,
                    cancellationToken
                )
            );
        }
    }
}
