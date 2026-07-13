using Microsoft.AspNetCore.Mvc;

namespace SalonCRM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { message = "Settings endpoint placeholder" });
}
