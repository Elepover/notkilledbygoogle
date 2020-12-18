using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NotKilledByGoogle.Bot.Grave.Helpers
{
    public class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) =>
            DateTimeOffset.ParseExact(Utils.ThrowIfNull(reader.GetString()),
                "yyyy-MM-dd", CultureInfo.InvariantCulture);

        public override void Write(
            Utf8JsonWriter writer,
            DateTimeOffset value,
            JsonSerializerOptions options) =>
            writer.WriteStringValue(value.ToString(
                "yyyy-MM-dd", CultureInfo.InvariantCulture));
    }
}
