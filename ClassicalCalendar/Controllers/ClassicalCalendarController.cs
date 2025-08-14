using DTO;
using Microsoft.AspNetCore.Mvc;

namespace ClassicalCalendar.Controllers;

public class ClassicalCalendarController : ControllerBase
{
    private readonly ILogger<ClassicalCalendarController> _logger;

    public ClassicalCalendarController(
        ILogger<ClassicalCalendarController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet(Name = "NewOrder")]
    public void ExecuteNewOrder()
    {
        return;
    }
}
