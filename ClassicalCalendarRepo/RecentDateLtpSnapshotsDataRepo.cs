using ClassicalCalendarGenericModel;
using DBModel.ClassicCalendarDbContext;
using DTO;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ClassicalCalendarRepo;

public class RecentDateLtpSnapshotsDataRepo
{
    private readonly ILogger<RecentDateLtpSnapshotsDataRepo> _logger;
    private readonly ClassicalCalendarContext _classicalCalendarContext;

    public RecentDateLtpSnapshotsDataRepo(
        ClassicalCalendarContext classicalCalendarContext,
        ILogger<RecentDateLtpSnapshotsDataRepo> logger)
    {
        _classicalCalendarContext = classicalCalendarContext ?? throw new ArgumentNullException(nameof(classicalCalendarContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Responses<List<LtpSnapshotDto>>> GetRecentDateLtpSnapshotsData()
    {
        var ltpSnapshotsDate = await _classicalCalendarContext.LtpSnapshots
            .OrderByDescending(c => c.SnapshotDate)
            .FirstOrDefaultAsync();

        if (ltpSnapshotsDate == null)
        {
            return Responses<List<LtpSnapshotDto>>.Error(HttpStatusCode.NotFound, "Ltp snapshot not found");
        }

        var ltpSnapshots = await _classicalCalendarContext.LtpSnapshots
            .Where(c => c.SnapshotDate == ltpSnapshotsDate.SnapshotDate)
            .ToListAsync();

        if (ltpSnapshots is not null && ltpSnapshots.Any())
        {
            return Responses<List<LtpSnapshotDto>>
                .Success(ltpSnapshots.Select(s => new LtpSnapshotDto
                {
                    Id = s.Id,
                    MonthlyCalendarId = s.MonthlyCalendarId,
                    CallBuyLTP = s.CallBuyLTP,
                    CallSellLTP = s.CallSellLTP,
                    PutBuyLTP = s.PutBuyLTP,
                    PutSellLTP = s.PutSellLTP,
                    SnapshotDate = s.SnapshotDate,
                    SnapshotTime = s.SnapshotTime
                }).ToList());
        }

        return Responses<List<LtpSnapshotDto>>.Error(HttpStatusCode.NotFound, "Ltp snapshot not found");
    }
}
