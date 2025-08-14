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
        var newOrder = new MonthlyCalendar
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
        };

        _ = await _classicalCalendarContext.MonthlyCalendars
            .AddAsync(newOrder);

        _ = await _classicalCalendarContext.SaveChangesAsync();

        _logger.LogInformation("New classical calendar order executed");

        return Responses<Empty>.Success(Empty.Instance, "New classical calendar order executed");
    }
}
