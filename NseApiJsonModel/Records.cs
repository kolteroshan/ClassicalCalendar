using System.Text.Json.Serialization;

namespace NseApiJsonModel;

public class Records
{
    [JsonPropertyName("expiryDates")]
    [JsonConverter(typeof(DateOnlyListConverter))]
    public List<DateOnly> ExpiryDates { get; set; }

    [JsonPropertyName("strikePrices")]
    public List<double> StrikePrices { get; set; }

    [JsonPropertyName("underlyingValue")]
    public double UnderlyingValue { get; set; }

    [JsonPropertyName("timestamp")]
    public string _dateAndTimeStamp { get; set; }

    [JsonPropertyName("data")]
    public List<Data> Data { get; set; }

    public DateOnly Date
    {
        get
        {
            if (DateTime.TryParseExact(_dateAndTimeStamp,
                "dd-MMM-yyyy HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime parsedDateTime))
            {
                return DateOnly.FromDateTime(parsedDateTime);
            }

            return DateOnly.MinValue;
        }
    }

    public TimeOnly Time
    {
        get
        {
            if (DateTime.TryParseExact(_dateAndTimeStamp,
                "dd-MMM-yyyy HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime parsedDateTime))
            {
                return TimeOnly.FromDateTime(parsedDateTime);
            }

            return TimeOnly.MinValue;
        }
    }
}