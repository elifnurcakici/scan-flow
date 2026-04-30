using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(new
        {
            message = "Backend çalışıyor",
            time = DateTime.UtcNow,
            day = DateTime.UtcNow.DayOfWeek
        });
    }
}