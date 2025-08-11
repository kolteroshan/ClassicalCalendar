namespace DBModel;

public class MonthlyCalendar
{
    public Guid Id { get; set; }

    public int Strike {  get; set; }

    public DateOnly ExecutionDate { get; set; }

    public TimeOnly ExecutionTime { get; set; }

    public DateOnly SellOrderExpiryDate { get; set; }

    public DateOnly BuyOrderExpiryDate { get; set; }

    public double CallSellLTP { get; set; }
    public double PutSellLTP { get; set; }
    public double CallBuyLTP { get; set; }
    public double PutBuyLTP { get; set; }

    public virtual ICollection<LtpSnapshot> LtpHistory { get; set; } = new List<LtpSnapshot>();
}
