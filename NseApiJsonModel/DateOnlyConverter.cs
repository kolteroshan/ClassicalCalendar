using System.Text.Json.Serialization;
using System.Text.Json;

namespace NseApiJsonModel;

public class DateOnlyConverter : JsonConverter<DateOnly>
{
    private readonly string _inputFormat = "dd-MMM-yyyy"; // input format (e.g. 21-Aug-2025)
    private readonly string _outputFormat = "dd-MM-yyyy"; // desired output format (e.g. 21-08-2025)

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
        {
            return DateOnly.MinValue; // fallback
        }

        // Always parse with exact input format
        return DateOnly.ParseExact(value, _inputFormat, System.Globalization.CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        // Always write in desired output format
        writer.WriteStringValue(value.ToString(_outputFormat));
    }
}