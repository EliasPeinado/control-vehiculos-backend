using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControlVehiculos.Models.DTOs.Evaluaciones;
using ControlVehiculos.Services.Interfaces;

namespace ControlVehiculos.Controllers;

/// <summary>
/// Controller para registro y consulta de evaluaciones técnicas
/// </summary>
[ApiController]
[Route("v1/evaluaciones")]
[Authorize]
public class EvaluacionesController : ControllerBase
{
    private readonly IEvaluacionService _evaluacionService;

    public EvaluacionesController(IEvaluacionService evaluacionService)
    {
        _evaluacionService = evaluacionService;
    }

    /// <summary>
    /// Registra una nueva evaluación técnica con sus 8 puntos de chequeo
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EvaluacionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterEvaluacionRequest request)
    {
        var evaluacion = await _evaluacionService.RegisterAsync(request);
        return CreatedAtAction(nameof(GetById), new { evaluacionId = evaluacion.Id }, evaluacion);
    }

    /// <summary>
    /// Obtiene una evaluación por su ID
    /// </summary>
    [HttpGet("{evaluacionId}")]
    [ProducesResponseType(typeof(EvaluacionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid evaluacionId)
    {
        var evaluacion = await _evaluacionService.GetByIdAsync(evaluacionId);
        if (evaluacion == null)
        {
            return NotFound();
        }
        return Ok(evaluacion);
    }
}
