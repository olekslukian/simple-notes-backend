using Microsoft.AspNetCore.Mvc;

namespace SimpleNotesApp.API.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
  [HttpGet]
  public IActionResult Get()
  {
    return Ok(new
    {
      status = "healthy",
      timestamp = DateTime.UtcNow,
      service = "SimpleNotesApp.API"
    });
  }
}
