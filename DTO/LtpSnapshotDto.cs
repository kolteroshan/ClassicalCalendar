namespace DTO;

public class LtpSnapshotDTO
{
    public Guid Id { get; set; }
    public Guid MonthlyCalendarId { get; set; }
    public DateOnly SnapshotDate { get; set; }
    public TimeOnly SnapshotTime { get; set; }
    public decimal CallSellLTP { get; set; }
    public decimal PutSellLTP { get; set; }
    public decimal CallBuyLTP { get; set; }
    public decimal PutBuyLTP { get; set; }
    public decimal Point { get; set; }
}
