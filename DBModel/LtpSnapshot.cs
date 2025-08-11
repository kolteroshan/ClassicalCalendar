namespace DBModel;

public class LtpSnapshot
{
    public Guid Id { get; set; }
    public Guid MonthlyCalendarId { get; set; }
    public MonthlyCalendar MonthlyCalendar { get; set; }
    public DateOnly SnapshotDate { get; set; }
    public TimeOnly SnapshotTime { get; set; }
    public double CallSellLTP { get; set; }
    public double PutSellLTP { get; set; }
    public double CallBuyLTP { get; set; }
    public double PutBuyLTP { get; set; }
}
