using ClassicalCalendarRepo;
using DBModel.ClassicCalendarDbContext;
using Microsoft.Extensions.Logging;
using OptiChainScheduler.NseApiService.NseIndexApiService;
using System.Net;

namespace OptiChainScheduler.BackGroundJobs;

public class ClassicalCalendarJobs
{
    private readonly ClassicalCalendarContext _classicalCalendarContext;
    private readonly NseIndexApiService _nseIndexApiService;
    private readonly ActiveClassicalCalendarRepo _activeClassicalCalendarRepo;
    private readonly ILogger<ClassicalCalendarJobs> _logger;

    public ClassicalCalendarJobs(
        ClassicalCalendarContext classicalCalendarContext,
        NseIndexApiService nseIndexApiService,
        ActiveClassicalCalendarRepo activeClassicalCalendarRepo,
        ILogger<ClassicalCalendarJobs> logger)
    {
        _classicalCalendarContext = classicalCalendarContext ?? throw new ArgumentNullException(nameof(classicalCalendarContext));
        _nseIndexApiService = nseIndexApiService ?? throw new ArgumentNullException(nameof(nseIndexApiService));
        _activeClassicalCalendarRepo = activeClassicalCalendarRepo ?? throw new ArgumentNullException(nameof(activeClassicalCalendarRepo));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task FetchCalendarData()
    {
        var activeStrike = await _activeClassicalCalendarRepo.GetActiveClassicalCalendrStrike();

        if (activeStrike is { StatusCode: HttpStatusCode.NotFound })
        {
            _logger.LogWarning(activeStrike.StatusMessage);
            return;
        }

        var data = await _nseIndexApiService.GetIndexOptionChainAsync("14-08-2025CE23050.00");
    }
}
