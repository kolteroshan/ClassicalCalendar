using ClassicalCalendarRepo;
using DTO;
using Enum;
using NseApi;
using NseApiStaticModel;
using System.Net;
using Records;

namespace OptiChainScheduler.BackGroundJobs;

public class ClassicalCalendarNewOrderJob
{
    private readonly NseIndexOptionChainStrikeApiService _nseIndexOptionChainStrikeApiService;
    private readonly ActiveClassicalCalendarRepo _activeClassicalCalendarRepo;
    private readonly ExecuteClassicalCalendarRepo _executeClassicalCalendarRepo;
    private readonly ILogger<ClassicalCalendarNewOrderJob> _logger;
    private readonly ZerodhaMarginCalculatorApiService _zerodhaMarginCalculatorApiService;

    public ClassicalCalendarNewOrderJob(
        NseIndexOptionChainStrikeApiService nseIndexOptionChainStrikeApiService,
        ActiveClassicalCalendarRepo activeClassicalCalendarRepo,
        ExecuteClassicalCalendarRepo executeClassicalCalendarRepo,
        ILogger<ClassicalCalendarNewOrderJob> logger,
        ZerodhaMarginCalculatorApiService zerodhaMarginCalculatorApiService)
    {
        _nseIndexOptionChainStrikeApiService = nseIndexOptionChainStrikeApiService ?? throw new ArgumentNullException(nameof(nseIndexOptionChainStrikeApiService));
        _activeClassicalCalendarRepo = activeClassicalCalendarRepo ?? throw new ArgumentNullException(nameof(activeClassicalCalendarRepo));
        _executeClassicalCalendarRepo = executeClassicalCalendarRepo ?? throw new ArgumentNullException(nameof(executeClassicalCalendarRepo));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _zerodhaMarginCalculatorApiService = zerodhaMarginCalculatorApiService ?? throw new ArgumentNullException(nameof(zerodhaMarginCalculatorApiService));
    }

    public async Task ExecuteNewOrder()
    {
        //await _zerodhaMarginCalculatorApiService.GetMargin("NFO", "OPT", "NIFTY", new DateOnly(2025, 9, 2), "PE", 24600, 75, "sell");

        var activeStrike = await _activeClassicalCalendarRepo.GetActiveClassicalCalendrStrike();

        if (activeStrike is { StatusCode: HttpStatusCode.OK })
        {
            return;
        }

        var today = DateTime.Now;

        if (today.DayOfWeek == DayOfWeek.Saturday || today.DayOfWeek == DayOfWeek.Sunday)
        {
            Console.WriteLine($"Today is {today.DayOfWeek}!");
            return;
        }

        _ = await _executeClassicalCalendarRepo
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

        var margin = await CalculateMargin(
            nextMonthOption.ExpiryDate, 
            (int)strike, 
            75, 
            buyStrike.CallDTO.LastPrice, 
            buyStrike.PutData.LastPrice);

        return new NewMonthlyCalendarDTO
        {
            Id = Guid.NewGuid(),
            BuyOrderExpiryDate = nextMonthAfterNextOption.ExpiryDate,
            SellOrderExpiryDate = nextMonthOption.ExpiryDate,
            CallBuyLTP = buyStrike.CallDTO.LastPrice,
            CallSellLTP = sellStrike.CallDTO.LastPrice,
            PutSellLTP = sellStrike.PutData.LastPrice,
            PutBuyLTP = buyStrike.PutData.LastPrice,
            Strike = (int)strike,
            ClassicalCalendarStatus = ClassicalCalendarStatus.Active,
            ExecutionDate = DateOnly.FromDateTime(now),
            ExecutionTime = TimeOnly.FromDateTime(now),
            TotalUsedMoney = margin.TotalUsedMoney,
            UsedMoneyForSell = margin.UsedMoneyForSell,
            UsedMoneyForBuy = margin.UsedMoneyForBuy,
            HedgeMoney = margin.HedgeMoney
        };
    }

    public async Task<MarginRecord> CalculateMargin(
        DateOnly sellExpiry,
        int strike, 
        int quentity,
        decimal ceBuyLtp,
        decimal peBuyLtp)
    {
        var ceSellMargin = await _zerodhaMarginCalculatorApiService.GetMargin("NFO", "OPT", "NIFTY", sellExpiry, "CE", strike, quentity, "sell");
        var peSellMargin = await _zerodhaMarginCalculatorApiService.GetMargin("NFO", "OPT", "NIFTY", sellExpiry, "PE", strike, quentity, "sell");

        var ceBuyMargin = ceBuyLtp * quentity;
        var peBuyMargin = peBuyLtp * quentity;

        var totalSell = ceSellMargin.Data.Total + peSellMargin.Data.Total;
        var totalBuy = ceBuyMargin + peBuyMargin;

        var reducedSellMargin = totalSell * 0.25m; // keep only 25% exposure
        var netMargin = reducedSellMargin + totalBuy;

        var hedge = totalSell - netMargin;

        return new MarginRecord(netMargin, totalSell, totalBuy, hedge);
    }
}
