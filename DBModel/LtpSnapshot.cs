using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBModel;

[Table("LtpSnapshots")]
public class LtpSnapshot
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid MonthlyCalendarId { get; set; }

    [ForeignKey(nameof(MonthlyCalendarId))]
    [InverseProperty(nameof(MonthlyCalendar.LtpHistory))]
    public MonthlyCalendar MonthlyCalendar { get; set; }

    [Required]
    public DateOnly SnapshotDate { get; set; }

    [Required]
    public TimeOnly SnapshotTime { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CallSellLTP { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PutSellLTP { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CallBuyLTP { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PutBuyLTP { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Point { get; set; }
}
