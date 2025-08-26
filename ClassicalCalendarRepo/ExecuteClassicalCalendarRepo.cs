using ClassicalCalendarGenericModel;
using DBModel;
using DBModel.ClassicCalendarDbContext;
using DTO;
using Enum;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ClassicalCalendarRepo;

public class ExecuteClassicalCalendarRepo
{
    private readonly ILogger<ExecuteClassicalCalendarRepo> _logger;
    private readonly ClassicalCalendarContext _classicalCalendarContext;

    public ExecuteClassicalCalendarRepo(
        ClassicalCalendarContext classicalCalendarContext,
        ILogger<ExecuteClassicalCalendarRepo> logger)
    {
        _classicalCalendarContext = classicalCalendarContext ?? throw new ArgumentNullException(nameof(classicalCalendarContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Responses<Empty>> ExecuteNewClassicalCalendar(NewMonthlyCalendarDTO newMonthlyCalendarDTO)
    {
        _ = await _classicalCalendarContext.MonthlyCalendars
            .AddAsync(new MonthlyCalendar
            {
                Id = newMonthlyCalendarDTO.Id,
                BuyOrderExpiryDate = newMonthlyCalendarDTO.BuyOrderExpiryDate,
                SellOrderExpiryDate = newMonthlyCalendarDTO.SellOrderExpiryDate,
                CallBuyLTP = newMonthlyCalendarDTO.CallBuyLTP,
                CallSellLTP = newMonthlyCalendarDTO.CallSellLTP,
                PutSellLTP = newMonthlyCalendarDTO.PutSellLTP,
                PutBuyLTP = newMonthlyCalendarDTO.PutBuyLTP,
                Strike = newMonthlyCalendarDTO.Strike,
                ClassicalCalendarStatus = newMonthlyCalendarDTO.ClassicalCalendarStatus,
                ExecutionDate = newMonthlyCalendarDTO.ExecutionDate,
                ExecutionTime = newMonthlyCalendarDTO.ExecutionTime
            });

        _ = await _classicalCalendarContext.SaveChangesAsync();

        _logger.LogInformation("New classical calendar order executed");

        return Responses<Empty>.Success(Empty.Instance, "New classical calendar order executed");
    }
}
