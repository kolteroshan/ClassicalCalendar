using System.Text.Json.Serialization;
using System.Text.Json;

namespace ClassicalCalendarJsonModel;

public class GrapthDataConverter : JsonConverter<List<GrapthPoint>>
{
    public override List<GrapthPoint> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var result = new List<GrapthPoint>();

        // Start of array
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException();

        reader.Read();

        while (reader.TokenType != JsonTokenType.EndArray)
        {
            // Each entry is an array [timestamp, value]
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException();

            reader.Read();
            long timestampMs = reader.GetInt64();
            reader.Read();
            decimal value = reader.GetDecimal();
            reader.Read();

            // Convert timestamp to DateOnly & TimeOnly
            var dt = DateTimeOffset.FromUnixTimeMilliseconds(timestampMs).ToLocalTime().DateTime;
            var point = new GrapthPoint
            {
                Date = DateOnly.FromDateTime(dt),
                Time = TimeOnly.FromDateTime(dt),
                Value = value
            };

            result.Add(point);

            if (reader.TokenType != JsonTokenType.EndArray)
                throw new JsonException();

            reader.Read();
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, List<GrapthPoint> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteEndArray();
    }
}