using Hangfire;
using OptiChainScheduler.BackGroundJobs;

namespace OptiChainScheduler
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly ClassicalCalendarJobs _jobs;

        public Worker(
            ILogger<Worker> logger,
            IRecurringJobManager recurringJobManager,
            ClassicalCalendarJobs jobs)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _recurringJobManager = recurringJobManager ?? throw new ArgumentNullException(nameof(recurringJobManager));
            _jobs = jobs ?? throw new ArgumentNullException(nameof(jobs));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _recurringJobManager.AddOrUpdate(
                "weekday-3-35pm",
                () => _jobs.FetchCalendarData(),
                "35 15 * * 1-5"
            );

            // Job Id = "weekday-3-35pm" - Unique
            // 35 → minute = 35
            // 15 → hour = 15(3 PM)
            // * → day of month = every day
            // * → month = every month
            // 1 - 5 → days of week = Monday through Friday(0 = Sunday, 6 = Saturday)
        }
    }
}
