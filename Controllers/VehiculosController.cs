using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControlVehiculos.Models.DTOs.Vehiculos;
using ControlVehiculos.Services.Interfaces;

namespace ControlVehiculos.Controllers;

/// <summary>
/// Controller para gestión de vehículos
/// </summary>
[ApiController]
[Route("v1/vehiculos")]
[Authorize]  // Todos los endpoints requieren autenticación
public class VehiculosController : ControllerBase
{
    private readonly IVehiculoService _vehiculoService;

    public VehiculosController(IVehiculoService vehiculoService)
    {
        _vehiculoService = vehiculoService;
    }

    /// <summary>
    /// Crea un nuevo vehículo
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(VehiculoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateVehiculoRequest request)
    {
        var vehiculo = await _vehiculoService.CreateAsync(request);
        return CreatedAtAction(nameof(GetByMatricula), new { matricula = vehiculo.Matricula }, vehiculo);
    }

    /// <summary>
    /// Obtiene un vehículo por su matrícula
    /// </summary>
    [HttpGet("{matricula}")]
    [ProducesResponseType(typeof(VehiculoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByMatricula(string matricula)
    {
        var vehiculo = await _vehiculoService.GetByMatriculaAsync(matricula);
        return Ok(vehiculo);
    }

    /// <summary>
    /// Actualiza los datos de un vehículo
    /// </summary>
    [HttpPut("{matricula}")]
    [ProducesResponseType(typeof(VehiculoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(string matricula, [FromBody] UpdateVehiculoRequest request)
    {
        var vehiculo = await _vehiculoService.UpdateAsync(matricula, request);
        return Ok(vehiculo);
    }

    /// <summary>
    /// Obtiene el historial de evaluaciones de un vehículo
    /// </summary>
    [HttpGet("{matricula}/evaluaciones")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEvaluaciones(
        string matricula,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _vehiculoService.GetEvaluacionesByMatriculaAsync(matricula, page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Verifica si existe un vehículo con la matrícula especificada
    /// </summary>
    [HttpHead("{matricula}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Exists(string matricula)
    {
        var exists = await _vehiculoService.ExistsByMatriculaAsync(matricula);
        return exists ? Ok() : NotFound();
    }
}
