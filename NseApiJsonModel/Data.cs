using System.Text.Json.Serialization;

namespace NseApiJsonModel;

public class Data
{
    [JsonPropertyName("expiryDate")]
    [JsonConverter(typeof(DateOnlyConverter))]
    public DateOnly ExpiryDate { get; set; }

    [JsonPropertyName("strikePrice")]
    public double StrikePrice { get; set; }

    [JsonPropertyName("PE")]
    public PutDetails PE { get; set; }

    [JsonPropertyName("CE")]
    public CallDetails CE { get; set; }
}