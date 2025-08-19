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

    [HttpGet(Name = "ActiveCalendar")]
    public async Task<IActionResult> ActiveCalendar() =>
        Ok(await _activeCalendarService.GetActiveCalendar());
}
