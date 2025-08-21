using Hangfire;
using OptiChainScheduler.BackGroundJobs;

namespace OptiChainScheduler.BackgroundExecutorService;

public class DataFetchWorker : BackgroundService
{
    private readonly ILogger<DataFetchWorker> _logger;
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly IServiceScopeFactory _scopeFactory;

    public DataFetchWorker(
        ILogger<DataFetchWorker> logger,
        IRecurringJobManager recurringJobManager,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _recurringJobManager = recurringJobManager ?? throw new ArgumentNullException(nameof(recurringJobManager));
        _scopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var jobs = scope.ServiceProvider.GetRequiredService<ClassicalCalendarJobs>();

        _recurringJobManager.AddOrUpdate(
            "weekday-3-35pm",
            () => jobs.FetchCalendarData(),
            "* * * * *"
        //"35 15 * * 1-5"
        );

        // Job Id = "weekday-3-35pm" - Unique
        // 35 → minute = 35
        // 15 → hour = 15(3 PM)
        // * → day of month = every day
        // * → month = every month
        // 1 - 5 → days of week = Monday through Friday(0 = Sunday, 6 = Saturday)
    }
}
