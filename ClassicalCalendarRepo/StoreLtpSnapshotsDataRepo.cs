using ClassicalCalendarGenericModel;
using DBModel.ClassicCalendarDbContext;
using DBModel;
using DTO;
using Enum;
using Microsoft.Extensions.Logging;

namespace ClassicalCalendarRepo;

public class StoreLtpSnapshotsDataRepo
{
    private readonly ILogger<StoreLtpSnapshotsDataRepo> _logger;
    private readonly ClassicalCalendarContext _classicalCalendarContext;

    public StoreLtpSnapshotsDataRepo(
        ClassicalCalendarContext classicalCalendarContext,
        ILogger<StoreLtpSnapshotsDataRepo> logger)
    {
        _classicalCalendarContext = classicalCalendarContext ?? throw new ArgumentNullException(nameof(classicalCalendarContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Responses<Empty>> AddLtpSnapshotsDate(List<LtpSnapshotDto> snapshots)
    {
        var newSnapshots = snapshots.Select(s => new LtpSnapshot
        {
            Id = s.Id,
            SnapshotDate = s.SnapshotDate,
            SnapshotTime = s.SnapshotTime,
            MonthlyCalendarId = s.MonthlyCalendarId,
            CallSellLTP = s.CallSellLTP,
            PutSellLTP = s.PutSellLTP,
            CallBuyLTP = s.CallBuyLTP,
            PutBuyLTP = s.PutBuyLTP
        }).ToList();

        await _classicalCalendarContext.LtpSnapshots
            .AddRangeAsync(newSnapshots);

        _ = await _classicalCalendarContext.SaveChangesAsync();

        _logger.LogInformation("New Ltp Snapshots added");

        return Responses<Empty>.Success(Empty.Instance, "New Ltp Snapshots added");
    }
}
