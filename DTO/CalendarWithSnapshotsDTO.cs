namespace DTO;

public class CalendarWithSnapshotsDTO
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
    public virtual ICollection<LtpSnapshotDTO> LtpHistory { get; set; } = new List<LtpSnapshotDTO>();
}