using Microsoft.AspNetCore.Mvc;

namespace SalonCRM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() => Ok(Array.Empty<object>());

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id) => Ok(new { id });
}
