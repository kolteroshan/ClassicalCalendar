using Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBModel;

[Table("MonthlyCalendars")]
public class MonthlyCalendar
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public int Strike {  get; set; }

    [Required]
    public DateOnly ExecutionDate { get; set; }

    [Required]
    public TimeOnly ExecutionTime { get; set; }

    [Required]
    public DateOnly SellOrderExpiryDate { get; set; }

    [Required]
    public DateOnly BuyOrderExpiryDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CallSellLTP { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PutSellLTP { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CallBuyLTP { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PutBuyLTP { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalUsedMoney { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UsedMoneyForSell { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UsedMoneyForBuy { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal HedgeMoney { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public ClassicalCalendarStatus ClassicalCalendarStatus { get; set; }

    [InverseProperty(nameof(LtpSnapshot.MonthlyCalendar))]
    public virtual ICollection<LtpSnapshot> LtpHistory { get; set; } = new List<LtpSnapshot>();
}
