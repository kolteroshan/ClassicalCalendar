namespace ApiDTO;

public class CalendarWithSnapshotsApiDTO
{
    public Guid Id { get; set; }

    public int Strike { get; set; }

    public DateOnly ExecutionDate { get; set; }

    public TimeOnly ExecutionTime { get; set; }

    public DateOnly SellOrderExpiryDate { get; set; }

    public DateOnly BuyOrderExpiryDate { get; set; }

    public decimal CallSellLTP { get; set; }
    public decimal PutSellLTP { get; set; }
    public decimal CallBuyLTP { get; set; }
    public decimal PutBuyLTP { get; set; }
    public decimal TotalUsedMoney { get; set; }
    public decimal UsedMoneyForSell { get; set; }
    public decimal UsedMoneyForBuy { get; set; }
    public decimal HedgeMoney { get; set; }
    public decimal BreakEvenAbove { get; set; }
    public decimal BreakEvenBelow { get; set; }
    public int SafetyBreakEvenAbove { get; set; }
    public int SafetyBreakEvenBelow { get; set; }
    public int DaysToLeftExpiry { get; set; }
    public int DaysToExpiry { get; set; }
    public double MaxProfit { get; set; }
    public double MinProfit { get; set; }
    public List<ProfitLossRange> ProfitLossRanges { get; set; }
    public virtual ICollection<LtpSnapshotApiDTO> LtpHistory { get; set; } = new List<LtpSnapshotApiDTO>();
}
