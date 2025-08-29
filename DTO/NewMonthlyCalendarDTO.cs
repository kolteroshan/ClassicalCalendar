using Enum;

namespace DTO;

public class NewMonthlyCalendarDTO
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int Strike { get; set; }

    public DateOnly ExecutionDate { get; set; } = new DateOnly();

    public TimeOnly ExecutionTime { get; set; } = new TimeOnly();

    public DateOnly SellOrderExpiryDate { get; set; }

    public DateOnly BuyOrderExpiryDate { get; set; }

    public decimal CallSellLTP { get; set; }
    public decimal PutSellLTP { get; set; }
    public decimal CallBuyLTP { get; set; }
    public decimal PutBuyLTP { get; set; }
    public ClassicalCalendarStatus ClassicalCalendarStatus { get; set; }
}
