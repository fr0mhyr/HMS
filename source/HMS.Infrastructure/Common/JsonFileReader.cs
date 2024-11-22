using System.Text.Json;
using System.Text.Json.Serialization;

namespace HMS.Infrastructure.Common;

public class JsonFileReader
{
    public static T? Read<T>(string fileName)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new CustomDateTimeConverter()
            }
        };

        using FileStream stream = File.OpenRead(fileName);
        return JsonSerializer.Deserialize<T>(stream, options);
    }

    public class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        private const string Format = "yyyyMMdd";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Unexpected token parsing date. Expected String, got {reader.TokenType}.");
            }

            var dateString = reader.GetString();
            if (DateTime.TryParseExact(dateString, Format, null, System.Globalization.DateTimeStyles.None,
                    out var date))
            {
                return date;
            }

            throw new JsonException($"Unable to parse '{dateString}' as a date in the format {Format}.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format));
        }
    }
}