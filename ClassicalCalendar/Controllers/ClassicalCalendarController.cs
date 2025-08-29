using ApiDTO;
using ClassicalCalendarServices;
using DTO;
using Microsoft.AspNetCore.Mvc;

namespace ClassicalCalendar.Controllers;

public class ClassicalCalendarController : ControllerBase
{
    private readonly ILogger<ClassicalCalendarController> _logger;
    private readonly ActiveCalendarService _activeCalendarService;


    public ClassicalCalendarController(
        ILogger<ClassicalCalendarController> logger,
        ActiveCalendarService activeCalendarService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _activeCalendarService = activeCalendarService ?? throw new ArgumentNullException(nameof(activeCalendarService));
    }

    [HttpGet(Name = "Calendar")]
    public async Task<IActionResult> GetCalendar(MonthCalendarApiDTO CalendarMonthDTO) =>
        Ok(await _activeCalendarService.GetCalendarByMonth(CalendarMonthDTO));

}
