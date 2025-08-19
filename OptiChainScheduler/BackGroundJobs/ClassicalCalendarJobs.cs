using ClassicalCalendarRepo;
using DTO;
using NseApi;
using System.Net;

namespace OptiChainScheduler.BackGroundJobs;

public class ClassicalCalendarJobs
{
    private readonly NseIndexApiService _nseIndexApiService;
    private readonly ActiveClassicalCalendarRepo _activeClassicalCalendarRepo;
    private readonly RecentDateLtpSnapshotsDataRepo _recentDateLtpSnapshotsDataRepo;
    private readonly ILogger<ClassicalCalendarJobs> _logger;
    private readonly StoreLtpSnapshotsDataRepo _storeLtpSnapshotsDataRepo;

    public ClassicalCalendarJobs(
        NseIndexApiService nseIndexApiService,
        ActiveClassicalCalendarRepo activeClassicalCalendarRepo,
        RecentDateLtpSnapshotsDataRepo recentDateLtpSnapshotsDataRepo,
        StoreLtpSnapshotsDataRepo storeLtpSnapshotsDataRepo,
        ILogger<ClassicalCalendarJobs> logger)
    {
        _nseIndexApiService = nseIndexApiService ?? throw new ArgumentNullException(nameof(nseIndexApiService));
        _activeClassicalCalendarRepo = activeClassicalCalendarRepo ?? throw new ArgumentNullException(nameof(activeClassicalCalendarRepo));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _recentDateLtpSnapshotsDataRepo = recentDateLtpSnapshotsDataRepo ?? throw new ArgumentNullException(nameof(recentDateLtpSnapshotsDataRepo));
        _storeLtpSnapshotsDataRepo = storeLtpSnapshotsDataRepo ?? throw new ArgumentNullException(nameof(storeLtpSnapshotsDataRepo));
    }

    public async Task FetchCalendarData()
    {
        var activeStrike = await _activeClassicalCalendarRepo.GetActiveClassicalCalendrStrike();

        if (activeStrike is { StatusCode: HttpStatusCode.NotFound })
        {
            _logger.LogWarning(activeStrike.StatusMessage);
            return;
        }

        var ceSellData = await _nseIndexApiService.GetIndexOptionChainAsync("30-09-2025CE24900.00");
        var peSellData = await _nseIndexApiService.GetIndexOptionChainAsync("30-09-2025PE24900.00");
        var ceBuyData = await _nseIndexApiService.GetIndexOptionChainAsync("28-10-2025CE24900.00");
        var peBuyData = await _nseIndexApiService.GetIndexOptionChainAsync("28-10-2025PE24900.00");

        var allTimeFrame = GetAllTimeFrames(ceSellData.Data, peSellData.Data, ceBuyData.Data, peBuyData.Data);

        var todayLtpSnapshots = new List<LtpSnapshotDto>();

        if (DateOnly.FromDateTime(DateTime.Today) == activeStrike.Data!.ExecutionDate)
        {
            todayLtpSnapshots = FirstDayOfExecution(
                ceSellData.Data,
                peSellData.Data,
                ceBuyData.Data,
                peBuyData.Data,
                activeStrike.Data,
                allTimeFrame);
        }
        else
        {
            todayLtpSnapshots = await RemainingDaysOfExecution(
                ceSellData.Data,
                peSellData.Data,
                ceBuyData.Data,
                peBuyData.Data,
                activeStrike.Data,
                allTimeFrame);
        }

        _ = await _storeLtpSnapshotsDataRepo.AddLtpSnapshotsDate(todayLtpSnapshots);
    }

    private List<TimeOnly> GetAllTimeFrames(
        StrikeSnapshotDTO? ceSellData,
        StrikeSnapshotDTO? peSellData,
        StrikeSnapshotDTO? ceBuyData,
        StrikeSnapshotDTO? peBuyData) =>
            (ceSellData?.LtpDtos?.Select(c => c.Time) ?? Enumerable.Empty<TimeOnly>())
            .Concat(peSellData?.LtpDtos?.Select(c => c.Time) ?? Enumerable.Empty<TimeOnly>())
            .Concat(ceBuyData?.LtpDtos?.Select(c => c.Time) ?? Enumerable.Empty<TimeOnly>())
            .Concat(peBuyData?.LtpDtos?.Select(c => c.Time) ?? Enumerable.Empty<TimeOnly>())
            .Distinct()
            .Order()
            .ToList();

    private async Task<List<LtpSnapshotDto>> RemainingDaysOfExecution(
        StrikeSnapshotDTO? ceSellData,
        StrikeSnapshotDTO? peSellData,
        StrikeSnapshotDTO? ceBuyData,
        StrikeSnapshotDTO? peBuyData,
        ActiveClassicalCalendarDTO activeClassicalCalendarDTO,
        List<TimeOnly> timeOnlies)
    {
        var yesterdaySnapshots = await _recentDateLtpSnapshotsDataRepo
            .GetRecentDateLtpSnapshotsData();

        var lastLtpSnapshot = yesterdaySnapshots.Data!.OrderByDescending(c => c.SnapshotTime).First();

        var todayLtpSnapshots = new List<LtpSnapshotDto>();

        foreach (var snapshot in timeOnlies)
        {
            var currentTime = new LtpSnapshotDto();

            currentTime.Id = Guid.NewGuid();
            currentTime.MonthlyCalendarId = activeClassicalCalendarDTO.Id;
            currentTime.SnapshotTime = snapshot;

            if (ceSellData is not null && ceSellData.LtpDtos.Any(c => c.Time == snapshot))
            {
                currentTime.CallSellLTP = ceSellData.LtpDtos.Where(c => c.Time == snapshot).Single().Value;
                currentTime.SnapshotDate = ceSellData?.Date ?? new DateOnly();
            }
            else
            {
                if (todayLtpSnapshots.Count() > 0)
                {
                    currentTime.CallSellLTP = todayLtpSnapshots.OrderByDescending(c => c.SnapshotTime).First().CallSellLTP;
                }
                else
                {
                    currentTime.CallSellLTP = lastLtpSnapshot.CallSellLTP;
                }
                
            }

            if (peSellData is not null && peSellData.LtpDtos.Any(c => c.Time == snapshot))
            {
                currentTime.PutSellLTP = peSellData.LtpDtos.Where(c => c.Time == snapshot).Single().Value;
                currentTime.SnapshotDate = peSellData?.Date ?? new DateOnly();
            }
            else
            {
                if (todayLtpSnapshots.Count() > 0)
                {
                    currentTime.PutSellLTP = todayLtpSnapshots.OrderByDescending(c => c.SnapshotTime).First().PutSellLTP;
                }
                else
                {
                    currentTime.PutSellLTP = lastLtpSnapshot.PutSellLTP;
                }
            }

            if (ceBuyData is not null && ceBuyData.LtpDtos.Any(c => c.Time == snapshot))
            {
                currentTime.CallBuyLTP = ceBuyData.LtpDtos.Where(c => c.Time == snapshot).Single().Value;
                currentTime.SnapshotDate = ceBuyData?.Date ?? new DateOnly();
            }
            else
            {
                if (todayLtpSnapshots.Count() > 0)
                {
                    currentTime.CallBuyLTP = todayLtpSnapshots.OrderByDescending(c => c.SnapshotTime).First().CallBuyLTP;
                }
                else
                {
                    currentTime.CallBuyLTP = lastLtpSnapshot.CallBuyLTP;
                }
            }

            if (peBuyData is not null && peBuyData.LtpDtos.Any(c => c.Time == snapshot))
            {
                currentTime.PutBuyLTP = peBuyData.LtpDtos.Where(c => c.Time == snapshot).Single().Value;
                currentTime.SnapshotDate = peBuyData?.Date ?? new DateOnly();
            }
            else
            {
                if (todayLtpSnapshots.Count() > 0)
                {
                    currentTime.PutBuyLTP = todayLtpSnapshots.OrderByDescending(c => c.SnapshotTime).First().PutBuyLTP;
                }
                else
                {
                    currentTime.PutBuyLTP = lastLtpSnapshot.PutBuyLTP;
                }
            }

            currentTime.Point = (activeClassicalCalendarDTO.CallSellLTP - currentTime.CallSellLTP) +
                (activeClassicalCalendarDTO.PutSellLTP - currentTime.PutSellLTP) +
                (currentTime.CallBuyLTP - activeClassicalCalendarDTO.CallBuyLTP) +
                (currentTime.PutBuyLTP - activeClassicalCalendarDTO.PutBuyLTP);

            todayLtpSnapshots.Add(currentTime);
        }

        return todayLtpSnapshots;
    }

    private List<LtpSnapshotDto> FirstDayOfExecution(
        StrikeSnapshotDTO? ceSellData,
        StrikeSnapshotDTO? peSellData,
        StrikeSnapshotDTO? ceBuyData,
        StrikeSnapshotDTO? peBuyData,
        ActiveClassicalCalendarDTO activeClassicalCalendarDTO,
        List<TimeOnly> timeOnlies)
    {
        var todayLtpSnapshots = new List<LtpSnapshotDto>();

        var firstSnapshot = new LtpSnapshotDto
        {
            Id = Guid.NewGuid(),
            MonthlyCalendarId = activeClassicalCalendarDTO.Id,
            SnapshotDate = activeClassicalCalendarDTO.ExecutionDate,
            SnapshotTime = activeClassicalCalendarDTO.ExecutionTime,
            CallSellLTP = activeClassicalCalendarDTO.CallSellLTP,
            PutSellLTP = activeClassicalCalendarDTO.PutSellLTP,
            CallBuyLTP = activeClassicalCalendarDTO.CallBuyLTP,
            PutBuyLTP = activeClassicalCalendarDTO.PutBuyLTP,
            Point = 0
        };

        timeOnlies = timeOnlies
            .Where(t => t > activeClassicalCalendarDTO.ExecutionTime)
            .Order()
            .ToList();

        todayLtpSnapshots.Add(firstSnapshot);

        foreach (var snapshot in timeOnlies)
        {
            var currentTime = new LtpSnapshotDto();

            currentTime.Id = Guid.NewGuid();
            currentTime.MonthlyCalendarId = activeClassicalCalendarDTO.Id;
            currentTime.SnapshotTime = snapshot;

            if (ceSellData is not null && ceSellData.LtpDtos.Any(c => c.Time == snapshot))
            {
                currentTime.CallSellLTP = ceSellData.LtpDtos.Where(c => c.Time == snapshot).Single().Value;
                currentTime.SnapshotDate = ceSellData?.Date ?? new DateOnly();
            }
            else
            {
                currentTime.CallSellLTP = todayLtpSnapshots.OrderByDescending(c => c.SnapshotTime).First().CallSellLTP;
            }

            if (peSellData is not null && peSellData.LtpDtos.Any(c => c.Time == snapshot))
            {
                currentTime.PutSellLTP = peSellData.LtpDtos.Where(c => c.Time == snapshot).Single().Value;
                currentTime.SnapshotDate = peSellData?.Date ?? new DateOnly();
            }
            else
            {
                currentTime.PutSellLTP = todayLtpSnapshots.OrderByDescending(c => c.SnapshotTime).First().PutSellLTP;
            }

            if (ceBuyData is not null && ceBuyData.LtpDtos.Any(c => c.Time == snapshot))
            {
                currentTime.CallBuyLTP = ceBuyData.LtpDtos.Where(c => c.Time == snapshot).Single().Value;
                currentTime.SnapshotDate = ceBuyData?.Date ?? new DateOnly();
            }
            else
            {
                currentTime.CallBuyLTP = todayLtpSnapshots.OrderByDescending(c => c.SnapshotTime).First().CallBuyLTP;
            }

            if (peBuyData is not null && peBuyData.LtpDtos.Any(c => c.Time == snapshot))
            {
                currentTime.PutBuyLTP = peBuyData.LtpDtos.Where(c => c.Time == snapshot).Single().Value;
                currentTime.SnapshotDate = peBuyData?.Date ?? new DateOnly();
            }
            else
            {
                currentTime.PutBuyLTP = todayLtpSnapshots.OrderByDescending(c => c.SnapshotTime).First().PutBuyLTP;
            }

            currentTime.Point = (activeClassicalCalendarDTO.CallSellLTP - currentTime.CallSellLTP) +
                (activeClassicalCalendarDTO.PutSellLTP - currentTime.PutSellLTP) +
                (currentTime.CallBuyLTP - activeClassicalCalendarDTO.CallBuyLTP) +
                (currentTime.PutBuyLTP - activeClassicalCalendarDTO.PutBuyLTP);

            todayLtpSnapshots.Add(currentTime);
        }

        return todayLtpSnapshots;
    }
}
