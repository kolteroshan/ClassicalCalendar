using ClassicalCalendarRepo;
using DTO;
using Enum;
using NseApi;
using System.Net;

namespace OptiChainScheduler.BackGroundJobs;

public class ClassicalCalendarNewOrderJob
{
    private readonly NseIndexOptionChainStrikeApiService _nseIndexOptionChainStrikeApiService;
    private readonly ActiveClassicalCalendarRepo _activeClassicalCalendarRepo;
    private readonly ExecuteClassicalCalendarRepo _executeClassicalCalendarRepo;
    private readonly ILogger<ClassicalCalendarNewOrderJob> _logger;

    public ClassicalCalendarNewOrderJob(
        NseIndexOptionChainStrikeApiService nseIndexOptionChainStrikeApiService,
        ActiveClassicalCalendarRepo activeClassicalCalendarRepo,
        ExecuteClassicalCalendarRepo executeClassicalCalendarRepo,
        ILogger<ClassicalCalendarNewOrderJob> logger)
    {
        _nseIndexOptionChainStrikeApiService = nseIndexOptionChainStrikeApiService ?? throw new ArgumentNullException(nameof(nseIndexOptionChainStrikeApiService));
        _activeClassicalCalendarRepo = activeClassicalCalendarRepo ?? throw new ArgumentNullException(nameof(activeClassicalCalendarRepo));
        _executeClassicalCalendarRepo = executeClassicalCalendarRepo ?? throw new ArgumentNullException(nameof(executeClassicalCalendarRepo));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ExecuteNewOrder()
    {
        await _nseIndexOptionChainStrikeApiService.GetIndexOptionChainAsync();
        //var activeStrike = await _activeClassicalCalendarRepo.GetActiveClassicalCalendrStrike();

        //if (activeStrike is { StatusCode: HttpStatusCode.OK })
        //{
        //    return;
        //}

        //var newOrder = await _executeClassicalCalendarRepo.ExecuteNewClassicalCalendar(
        //    new NewMonthlyCalendarDTO
        //    {
        //        Id = Guid.NewGuid(),
        //        BuyOrderExpiryDate = new DateOnly(),
        //        SellOrderExpiryDate = new DateOnly(),
        //        CallBuyLTP = 100,
        //        CallSellLTP = 100,
        //        PutSellLTP = 100,
        //        PutBuyLTP = 100,
        //        Strike = 25000,
        //        ClassicalCalendarStatus = ClassicalCalendarStatus.Active,
        //        ExecutionDate = new DateOnly(),
        //        ExecutionTime = new TimeOnly()
        //    });

        //var data = await _nseIndexApiService.GetIndexOptionChainAsync("14-08-2025CE23050.00");
    }
}
