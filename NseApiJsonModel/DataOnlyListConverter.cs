using System.Text.Json.Serialization;
using System.Text.Json;

namespace NseApiJsonModel;

public class DateOnlyListConverter : JsonConverter<List<DateOnly>>
{
    private readonly string _inputFormat = "dd-MMM-yyyy"; // e.g. 30-Jun-2026
    private readonly string _outputFormat = "dd-MM-yyyy"; // e.g. 20-08-2025

    public override List<DateOnly> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dates = new List<DateOnly>();

        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType == JsonTokenType.String)
            {
                var str = reader.GetString();
                if (DateOnly.TryParseExact(str!, _inputFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var date))
                {
                    dates.Add(date);
                }
                else
                {
                    throw new JsonException($"Invalid date format: {str}");
                }
            }
        }

        return dates;
    }

    public override void Write(Utf8JsonWriter writer, List<DateOnly> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var date in value)
        {
            writer.WriteStringValue(date.ToString(_outputFormat));
        }
        writer.WriteEndArray();
    }
}