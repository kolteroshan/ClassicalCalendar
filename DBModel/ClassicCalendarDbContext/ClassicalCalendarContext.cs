using Microsoft.EntityFrameworkCore;

namespace DBModel.ClassicCalendarDbContext;

public class ClassicalCalendarContext : DbContext
{
    public ClassicalCalendarContext(DbContextOptions<ClassicalCalendarContext> options)
        : base(options)
    {
    }

    public DbSet<LtpSnapshot> LtpSnapshots { get; set; }

    public DbSet<MonthlyCalendar> MonthlyCalendars { get; set; }
}