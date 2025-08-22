using System.Text.Json.Serialization;

namespace NseApiJsonModel;

public class OptionChainData
{
    [JsonPropertyName("records")]
    public Records Records { get; set; }
}