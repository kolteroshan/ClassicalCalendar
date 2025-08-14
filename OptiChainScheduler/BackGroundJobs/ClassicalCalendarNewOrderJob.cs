using ClassicalCalendarRepo;
using DTO;
using Enum;
using OptiChainScheduler.NseApiService.NseIndexApiService;
using System.Net;

namespace OptiChainScheduler.BackGroundJobs;

public class ClassicalCalendarNewOrderJob
{
    private readonly NseIndexApiService _nseIndexApiService;
    private readonly ActiveClassicalCalendarRepo _activeClassicalCalendarRepo;
    private readonly ExecuteClassicalCalendarRepo _executeClassicalCalendarRepo;
    private readonly ILogger<ClassicalCalendarNewOrderJob> _logger;

    public ClassicalCalendarNewOrderJob(
        NseIndexApiService nseIndexApiService,
        ActiveClassicalCalendarRepo activeClassicalCalendarRepo,
        ExecuteClassicalCalendarRepo executeClassicalCalendarRepo,
        ILogger<ClassicalCalendarNewOrderJob> logger)
    {
        _nseIndexApiService = nseIndexApiService ?? throw new ArgumentNullException(nameof(nseIndexApiService));
        _activeClassicalCalendarRepo = activeClassicalCalendarRepo ?? throw new ArgumentNullException(nameof(activeClassicalCalendarRepo));
        _executeClassicalCalendarRepo = executeClassicalCalendarRepo ?? throw new ArgumentNullException(nameof(executeClassicalCalendarRepo));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ExecuteNewOrder()
    {
        var activeStrike = await _activeClassicalCalendarRepo.GetActiveClassicalCalendrStrike();

        if (activeStrike is { StatusCode: HttpStatusCode.OK })
        {
            return;
        }

        var newOrder = await _executeClassicalCalendarRepo.ExecuteNewClassicalCalendar(
            new NewMonthlyCalendarDTO
            {
                Id = Guid.NewGuid(),
                BuyOrderExpiryDate = new DateOnly(),
                SellOrderExpiryDate = new DateOnly(),
                CallBuyLTP = 100,
                CallSellLTP = 100,
                PutSellLTP = 100,
                PutBuyLTP = 100,
                Strike = 25000,
                ClassicalCalendarStatus = ClassicalCalendarStatus.Active,
                ExecutionDate = new DateOnly(),
                ExecutionTime = new TimeOnly()
            });

        var data = await _nseIndexApiService.GetIndexOptionChainAsync("14-08-2025CE23050.00");
    }
}
