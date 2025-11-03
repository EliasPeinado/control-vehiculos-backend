using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControlVehiculos.Models.DTOs.Turnos;
using ControlVehiculos.Services.Interfaces;

namespace ControlVehiculos.Controllers;

/// <summary>
/// Controller para gestión de turnos de inspección
/// </summary>
[ApiController]
[Route("v1/turnos")]
[Authorize]  // Todos los endpoints requieren autenticación
public class TurnosController : ControllerBase
{
    private readonly ITurnoService _turnoService;

    public TurnosController(ITurnoService turnoService)
    {
        _turnoService = turnoService;
    }

    /// <summary>
    /// Consulta disponibilidad de slots para un centro y fecha
    /// </summary>
    [HttpGet("slots")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSlots([FromQuery] Guid centroId, [FromQuery] DateTime fecha)
    {
        var slots = await _turnoService.GetSlotsAsync(centroId, fecha);
        return Ok(slots);
    }

    /// <summary>
    /// Crea una nueva reserva de turno
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TurnoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateTurnoRequest request)
    {
        var turno = await _turnoService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { turnoId = turno.Id }, turno);
    }

    /// <summary>
    /// Obtiene un turno por su ID
    /// </summary>
    [HttpGet("{turnoId}")]
    [ProducesResponseType(typeof(TurnoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid turnoId)
    {
        var turno = await _turnoService.GetByIdAsync(turnoId);
        if (turno == null)
        {
            return NotFound();
        }
        return Ok(turno);
    }

    /// <summary>
    /// Confirma un turno previamente reservado
    /// </summary>
    [HttpPost("{turnoId}/confirmar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Confirmar(Guid turnoId)
    {
        var result = await _turnoService.ConfirmarAsync(turnoId);
        return result ? NoContent() : NotFound();
    }

    /// <summary>
    /// Cancela un turno
    /// </summary>
    [HttpPost("{turnoId}/cancelar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancelar(Guid turnoId, [FromBody] CancelarTurnoRequest request)
    {
        var result = await _turnoService.CancelarAsync(turnoId, request.Motivo);
        return result ? NoContent() : NotFound();
    }
}
