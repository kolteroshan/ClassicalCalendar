using System.Text.Json.Serialization;

namespace ClassicalCalendarJsonModel;

public class OptionChainData
{
    [JsonPropertyName("records")]
    public Records Records { get; set; }
}