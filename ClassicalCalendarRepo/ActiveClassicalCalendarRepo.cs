using ClassicalCalendarGenericModel;
using DBModel.ClassicCalendarDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using Enum;
using DTO;

namespace ClassicalCalendarRepo;

public class ActiveClassicalCalendarRepo
{
    private readonly ILogger<ActiveClassicalCalendarRepo> _logger;
    private readonly ClassicalCalendarContext _classicalCalendarContext;

    public ActiveClassicalCalendarRepo(
        ClassicalCalendarContext classicalCalendarContext,
        ILogger<ActiveClassicalCalendarRepo> logger)
    {
        _classicalCalendarContext = classicalCalendarContext ?? throw new ArgumentNullException(nameof(classicalCalendarContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Responses<ActiveClassicalCalendarDTO>> GetActiveClassicalCalendrStrike()
    {
        var activeStrike = await _classicalCalendarContext.MonthlyCalendars
            .SingleOrDefaultAsync(a => a.ClassicalCalendarStatus == ClassicalCalendarStatus.Active);

        if (activeStrike is not null)
        {
            return Responses<ActiveClassicalCalendarDTO>.Success(new ActiveClassicalCalendarDTO
            {
                Id = activeStrike.Id,
                Strike = activeStrike.Strike,
                SellOrderExpiryDate = activeStrike.SellOrderExpiryDate,
                BuyOrderExpiryDate = activeStrike.BuyOrderExpiryDate,
                ExecutionDate = activeStrike.ExecutionDate,
                ClassicalCalendarStatus = activeStrike.ClassicalCalendarStatus,
                ExecutionTime = activeStrike.ExecutionTime,
                PutBuyLTP = activeStrike.PutBuyLTP,
                PutSellLTP = activeStrike.PutSellLTP,
                CallBuyLTP = activeStrike.CallBuyLTP,
                CallSellLTP = activeStrike.CallSellLTP
            });
        }

        _logger.LogWarning("No any active trade");
        return Responses<ActiveClassicalCalendarDTO>.Error(HttpStatusCode.NotFound, "No any active trade");
    }
}