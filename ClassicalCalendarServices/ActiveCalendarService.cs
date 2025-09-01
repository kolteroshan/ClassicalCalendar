using DTO;
using ApiDTO;
using ClassicalCalendarGenericModel;
using ClassicalCalendarRepo;
using Microsoft.Extensions.Logging;
using Records;
using NseApi;
using NseApiStaticModel;

namespace ClassicalCalendarServices;

public class ActiveCalendarService
{
    private readonly ILogger<ActiveCalendarService> _logger;
    private readonly ClassicalCalendarDataRepo _classicalCalendarRepo;
    private readonly NseIndexOptionChainStrikeApiService _nseIndexOptionChainStrikeApiService;

    public ActiveCalendarService(
        ILogger<ActiveCalendarService> logger,
        ClassicalCalendarDataRepo classicalCalendarRepo,
        NseIndexOptionChainStrikeApiService nseIndexOptionChainStrikeApiService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _classicalCalendarRepo = classicalCalendarRepo ?? throw new ArgumentNullException(nameof(classicalCalendarRepo));
        _nseIndexOptionChainStrikeApiService = nseIndexOptionChainStrikeApiService ?? throw new ArgumentNullException(nameof(nseIndexOptionChainStrikeApiService));
    }

    public async Task<Responses<CalendarWithSnapshotsApiDTO>> GetCalendarByMonth(MonthCalendarApiDTO calendarMonthDTO)
    {
        var calendarData = await _classicalCalendarRepo.GetActiveClassicalCalendarData(
            new MonthCalendarDTO
            {
                Month = calendarMonthDTO.Month,
                Year = calendarMonthDTO.Year
            });

        var data = new CalendarWithSnapshotsApiDTO();

        data.Id = calendarData.Data.Id;
        data.Strike = calendarData.Data.Strike;
        data.ExecutionDate = calendarData.Data.ExecutionDate;
        data.ExecutionTime = calendarData.Data.ExecutionTime;
        data.SellOrderExpiryDate = calendarData.Data.SellOrderExpiryDate;
        data.BuyOrderExpiryDate = calendarData.Data.BuyOrderExpiryDate;
        data.CallSellLTP = calendarData.Data.CallSellLTP;
        data.PutSellLTP = calendarData.Data.PutSellLTP;
        data.CallBuyLTP = calendarData.Data.CallBuyLTP;
        data.PutBuyLTP = calendarData.Data.PutBuyLTP;
        data.TotalUsedMoney = calendarData.Data.TotalUsedMoney;
        data.UsedMoneyForSell = calendarData.Data.UsedMoneyForSell;
        data.UsedMoneyForBuy = calendarData.Data.UsedMoneyForBuy;
        data.HedgeMoney = calendarData.Data.HedgeMoney;

        var optionChain = await _nseIndexOptionChainStrikeApiService
            .GetIndexOptionChainAsync(NseIndexTypes.Nifty);

        var liveStrike = Math.Ceiling(optionChain.Data.Expires.First().LiveStrike);

        var bETRecord = GetBET(
            data.Strike,
            data.CallBuyLTP,
            data.PutBuyLTP,
            data.CallSellLTP,
            data.PutSellLTP,
            liveStrike);

        data.BreakEvenAbove = bETRecord.BreakEvenAbove;
        data.BreakEvenBelow = bETRecord.BreakEvenBelow;
        data.SafetyBreakEvenAbove = bETRecord.SafetyBreakEvenAbove;
        data.SafetyBreakEvenBelow = bETRecord.SafetyBreakEvenBelow;

        return Responses<CalendarWithSnapshotsApiDTO>.Success(data);
    }

    public BETRecord GetBET(
        decimal strike,
        decimal buyCE,
        decimal buyPE,
        decimal sellCE,
        decimal sellPE,
        decimal liveStrike)
    {
        var netDebit = (buyCE + buyPE) - (sellCE + sellPE);

        var upperBepStrike = strike + netDebit;
        var lowerBepStrike = strike - netDebit;

        var upperBepPoint = upperBepStrike - strike;
        var lowerBepPoint = strike - lowerBepStrike;

        var safetyBreakEvenUpperStrikePoint = liveStrike > strike ? upperBepPoint - (liveStrike - strike) : (strike - liveStrike) + upperBepPoint;
        var safetyBreakEvenLowerStrikePoint = liveStrike > strike ? lowerBepPoint - (liveStrike - strike) : (strike - liveStrike) + lowerBepPoint;

        return new BETRecord(upperBepPoint, lowerBepPoint, safetyBreakEvenUpperStrikePoint, safetyBreakEvenLowerStrikePoint);
    }

}
