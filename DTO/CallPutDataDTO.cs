using System.Text.Json.Serialization;

namespace DTO;

public class CallPutDataDTO
{
    public string CallUnderlying { get; set; }
    public string PutUnderlying { get; set; }
    public double CallOpenInterest { get; set; }
    public double PutOpenInterest { get; set; }
    public double CallChangeinOpenInterest { get; set; }
    public double PutChangeinOpenInterest { get; set; }
    public double CallPchangeinOpenInterest { get; set; }
    public double PutPchangeinOpenInterest { get; set; }
    public double CallTotalTradedVolume { get; set; }
    public double PutTotalTradedVolume { get; set; }
    public double CallLTP { get; set; }
    public double PutLTP { get; set; }
    public double CallBidprice { get; set; }
    public double PutBidprice { get; set; }
    public double CallAskPrice { get; set; }
    public double PutAskPrice { get; set; }
    public double CallUnderlyingValue { get; set; }
    public double PutUnderlyingValue { get; set; }
}