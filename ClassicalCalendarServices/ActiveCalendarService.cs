using DTO;
using ApiDTO;
using ClassicalCalendarGenericModel;
using ClassicalCalendarRepo;
using Microsoft.Extensions.Logging;

namespace ClassicalCalendarServices;

public class ActiveCalendarService
{
    private readonly ILogger<ActiveCalendarService> _logger;
    private readonly ClassicalCalendarDataRepo _classicalCalendarRepo;

    public ActiveCalendarService(
        ILogger<ActiveCalendarService> logger,
        ClassicalCalendarDataRepo classicalCalendarRepo)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _classicalCalendarRepo = classicalCalendarRepo ?? throw new ArgumentNullException(nameof(classicalCalendarRepo));
    }

    public async Task<Responses<CalendarWithSnapshotsDTO>> GetCalendarByMonth(MonthCalendarApiDTO calendarMonthDTO)
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
        data.TotalUsedMoney = ((calendarData.Data.CallBuyLTP - calendarData.Data.CallSellLTP)
            + (calendarData.Data.PutBuyLTP - calendarData.Data.PutSellLTP)) * 75;

        return calendarData;
    }
        
}
