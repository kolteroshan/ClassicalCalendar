using System.Text.Json.Serialization;

namespace NseApiJsonModel;

public class MarginData
{
    [JsonPropertyName("last")]
    public MarginDetail Last { get; set; }
}

public class MarginDetail
{
    [JsonPropertyName("span")]
    public decimal Span { get; set; }

    [JsonPropertyName("exposure")]
    public decimal Exposure { get; set; }

    [JsonPropertyName("netoptionvalue")]
    public decimal NetOptionValue { get; set; }

    [JsonPropertyName("spread")]
    public decimal Spread { get; set; }

    [JsonPropertyName("total")]
    public decimal Total { get; set; }
}