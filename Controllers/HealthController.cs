using Microsoft.AspNetCore.Mvc;
using ControlVehiculos.Models.DTOs;

namespace ControlVehiculos.Controllers;

[ApiController]
[Route("v1")]
public class HealthController : ControllerBase
{
    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        return Ok(new HealthResponse
        {
            Status = "Healthy",
            Time = DateTime.UtcNow
        });
    }
}
