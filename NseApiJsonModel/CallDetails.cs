using System.Text.Json.Serialization;

namespace NseApiJsonModel;

public class CallDetails
{
    [JsonPropertyName("expiryDate")]
    [JsonConverter(typeof(DateOnlyConverter))]
    public DateOnly ExpiryDate { get; set; }

    [JsonPropertyName("strikePrice")]
    public double StrikePrice { get; set; }

    [JsonPropertyName("underlying")]
    public string Underlying { get; set; }

    [JsonPropertyName("openInterest")]
    public double OpenInterest { get; set; }

    [JsonPropertyName("changeinOpenInterest")]
    public double ChangeinOpenInterest { get; set; }

    [JsonPropertyName("pchangeinOpenInterest")]
    public double PchangeinOpenInterest { get; set; }

    [JsonPropertyName("totalTradedVolume")]
    public double TotalTradedVolume { get; set; }

    [JsonPropertyName("lastPrice")]
    public decimal LastPrice { get; set; }

    [JsonPropertyName("bidprice")]
    public double Bidprice { get; set; }

    [JsonPropertyName("askPrice")]
    public double AskPrice { get; set; }

    [JsonPropertyName("underlyingValue")]
    public double UnderlyingValue { get; set; }
}