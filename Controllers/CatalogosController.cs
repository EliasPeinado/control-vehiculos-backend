using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControlVehiculos.Services.Interfaces;

namespace ControlVehiculos.Controllers;

[ApiController]
[Route("v1")]
[Authorize]  // Todos los endpoints requieren autenticación
public class CatalogosController : ControllerBase
{
    private readonly ICatalogoService _catalogoService;

    public CatalogosController(ICatalogoService catalogoService)
    {
        _catalogoService = catalogoService;
    }

    [HttpGet("chequeos")]
    public async Task<IActionResult> GetChequeos()
    {
        var chequeos = await _catalogoService.GetChequeosAsync();
        return Ok(chequeos);
    }

    [HttpGet("catalogos/estados-turno")]
    public async Task<IActionResult> GetEstadosTurno()
    {
        var estados = await _catalogoService.GetEstadosTurnoAsync();
        return Ok(estados);
    }

    [HttpGet("catalogos/estados-vehiculo")]
    public async Task<IActionResult> GetEstadosVehiculo()
    {
        var estados = await _catalogoService.GetEstadosVehiculoAsync();
        return Ok(estados);
    }

    [HttpGet("catalogos/resultados-evaluacion")]
    public async Task<IActionResult> GetResultadosEvaluacion()
    {
        var resultados = await _catalogoService.GetResultadosEvaluacionAsync();
        return Ok(resultados);
    }
}
