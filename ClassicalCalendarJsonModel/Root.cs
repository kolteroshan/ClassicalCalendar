using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ClassicalCalendarJsonModel;

public class Root
{
    [JsonPropertyName("identifier")]
    public string _identifier { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("grapthData")]
    [JsonConverter(typeof(GrapthDataConverter))]
    public List<GrapthPoint> GrapthData { get; set; }

    [JsonPropertyName("closePrice")]
    public decimal ClosePrice { get; set; }

    public string Strike
    {
        get
        {
            return Regex.Match(_identifier, @"(\d+)\.00$").Groups[1].Value;
        }
        set
        {
            _identifier = value;
        }
    }

    public string Type
    {
        get
        {
            return Regex.Match(_identifier, @"(CE|PE)").Value;
        }
        set
        {
            _identifier = value;
        }
    }
}
