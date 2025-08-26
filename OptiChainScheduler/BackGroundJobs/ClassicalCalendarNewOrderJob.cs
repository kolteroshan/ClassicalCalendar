using ClassicalCalendarRepo;
using DTO;
using Enum;
using NseApi;
using NseApiStaticModel;
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
        var activeStrike = await _activeClassicalCalendarRepo.GetActiveClassicalCalendrStrike();

        if (activeStrike is { StatusCode: HttpStatusCode.OK })
        {
            return;
        }

        var newOrder = await _executeClassicalCalendarRepo
            .ExecuteNewClassicalCalendar(await GetNewClassicalCalendarOrder());
    }

    public async Task<NewMonthlyCalendarDTO> GetNewClassicalCalendarOrder()
    {
        var optionChainData = await _nseIndexOptionChainStrikeApiService
            .GetIndexOptionChainAsync(NseIndexTypes.Nifty);

        var month = optionChainData.Data.DateOfOptionChain.Month;

        var now = DateTime.Now;

        var nextMonthOption = optionChainData.Data.Expires
            .Where(c => c.ExpiryDate.Month == now.AddMonths(1).Month)
            .OrderByDescending(c => c.ExpiryDate)
            .FirstOrDefault();

        var nextMonthAfterNextOption = optionChainData.Data.Expires
            .Where(c => c.ExpiryDate.Month == now.AddMonths(2).Month)
            .OrderByDescending(c => c.ExpiryDate)
            .FirstOrDefault();

        var strike = Math.Floor(nextMonthOption.LiveStrike / 100) * 100;

        var sellStrike = nextMonthOption.Strikes.Where(s => s.Strike == strike).Single();

        var buyStrike = nextMonthAfterNextOption.Strikes.Where(s => s.Strike == strike).Single();

        return new NewMonthlyCalendarDTO
        {
            Id = Guid.NewGuid(),
            BuyOrderExpiryDate = nextMonthAfterNextOption.ExpiryDate,
            SellOrderExpiryDate = nextMonthOption.ExpiryDate,
            CallBuyLTP = buyStrike.CallDTO.LastPrice,
            CallSellLTP = sellStrike.CallDTO.LastPrice,
            PutSellLTP = sellStrike.PutData.LastPrice,
            PutBuyLTP = buyStrike.PutData.LastPrice,
            Strike = strike,
            ClassicalCalendarStatus = ClassicalCalendarStatus.Active,
            ExecutionDate = DateOnly.FromDateTime(now),
            ExecutionTime = TimeOnly.FromDateTime(now)
        };
    }
}
