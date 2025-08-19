using ClassicalCalendarGenericModel;
using ClassicalCalendarRepo;
using DTO;
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

    public async Task<Responses<ActiveCalendarWithSnapshotsDTO>> GetActiveCalendar() =>
        await _classicalCalendarRepo.GetActiveClassicalCalendarData();
}
