using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NotKilledByGoogle.Bot.Grave
{
    public class DeceasedTypeConverter : JsonConverter<DeceasedType>
    {
        public override DeceasedType Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
            => Utils.ThrowIfNull(reader.GetString()) switch
            {
                "app" => DeceasedType.App,
                "service" => DeceasedType.Service,
                "hardware" => DeceasedType.Hardware,
                _ => DeceasedType.Other
            };

        public override void Write(
            Utf8JsonWriter writer,
            DeceasedType value,
            JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString("G").ToLowerInvariant());
    }
}
