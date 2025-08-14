using Hangfire;
using OptiChainScheduler.BackGroundJobs;

namespace OptiChainScheduler.BackgroundExecutorService;

public class ExecuteNewOrderWorker : BackgroundService
{
    private readonly ILogger<ExecuteNewOrderWorker> _logger;
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly IServiceScopeFactory _scopeFactory;

    public ExecuteNewOrderWorker(
        ILogger<ExecuteNewOrderWorker> logger,
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
        var jobs = scope.ServiceProvider.GetRequiredService<ClassicalCalendarNewOrderJob>();

        _recurringJobManager.AddOrUpdate(
            "weekday-11-11am",
            () => jobs.ExecuteNewOrder(),
           "11 11 * * 1-5"
        );
    }
}