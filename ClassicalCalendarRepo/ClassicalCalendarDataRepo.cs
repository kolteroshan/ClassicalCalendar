using ClassicalCalendarGenericModel;
using DBModel.ClassicCalendarDbContext;
using DBModel;
using DTO;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Enum;
using System.Net;

namespace ClassicalCalendarRepo;

public class ClassicalCalendarDataRepo
{
    private readonly ILogger<ClassicalCalendarDataRepo> _logger;
    private readonly ClassicalCalendarContext _classicalCalendarContext;

    public ClassicalCalendarDataRepo(
        ClassicalCalendarContext classicalCalendarContext,
        ILogger<ClassicalCalendarDataRepo> logger)
    {
        _classicalCalendarContext = classicalCalendarContext ?? throw new ArgumentNullException(nameof(classicalCalendarContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Responses<CalendarWithSnapshotsDTO>> GetActiveClassicalCalendarData(MonthCalendarDTO monthCalendarDTO)
    {
        var calendarDatas = await _classicalCalendarContext.MonthlyCalendars
            .SingleOrDefaultAsync(a => a.SellOrderExpiryDate.Month == monthCalendarDTO.Month && a.SellOrderExpiryDate.Year == monthCalendarDTO.Year);

        if (calendarDatas is null)
        {
            return Responses<CalendarWithSnapshotsDTO>.Error(HttpStatusCode.NotFound, "Calendar Not Found");
        }

        return Responses<CalendarWithSnapshotsDTO>.Success(new CalendarWithSnapshotsDTO
        {
            Id = calendarDatas.Id,
            ExecutionDate = calendarDatas.ExecutionDate,
            ExecutionTime = calendarDatas.ExecutionTime,
            SellOrderExpiryDate = calendarDatas.SellOrderExpiryDate,
            BuyOrderExpiryDate = calendarDatas.BuyOrderExpiryDate,
            Strike = calendarDatas.Strike,
            CallBuyLTP = calendarDatas.CallBuyLTP,
            PutBuyLTP = calendarDatas.PutBuyLTP,
            CallSellLTP = calendarDatas.CallSellLTP,
            PutSellLTP = calendarDatas.PutSellLTP,
            LtpHistory = calendarDatas.LtpHistory.Select(h => new LtpSnapshotDTO
            {
                Id = h.Id,
                SnapshotDate = h.SnapshotDate,
                SnapshotTime = h.SnapshotTime,
                MonthlyCalendarId = h.MonthlyCalendarId,
                CallSellLTP = h.CallSellLTP,
                PutSellLTP = h.PutSellLTP,
                CallBuyLTP = h.CallBuyLTP,
                PutBuyLTP = h.PutBuyLTP,
                Point = h.Point
            }).ToList()
        });
    }
}