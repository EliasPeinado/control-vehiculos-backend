using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControlVehiculos.Services.Interfaces;

namespace ControlVehiculos.Controllers;

[ApiController]
[Route("v1/centros")]
[Authorize]  // Todos los endpoints requieren autenticación
public class CentrosController : ControllerBase
{
    private readonly ICentroService _centroService;

    public CentrosController(ICentroService centroService)
    {
        _centroService = centroService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _centroService.GetAllAsync(page, pageSize);
        return Ok(result);
    }
}
