namespace NseApiDTO;

public class PutDTO
{
    public DateOnly ExpiryDate { get; set; }
    public double StrikePrice { get; set; }
    public string Underlying { get; set; }
    public double OpenInterest { get; set; }
    public double ChangeinOpenInterest { get; set; }
    public double PchangeinOpenInterest { get; set; }
    public double TotalTradedVolume { get; set; }
    public double LastPrice { get; set; }
    public double Bidprice { get; set; }
    public double AskPrice { get; set; }
    public double UnderlyingValue { get; set; }
}