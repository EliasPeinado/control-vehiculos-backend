using System.Security.Claims;
using ControlVehiculos.Exceptions;
using ControlVehiculos.Models.DTOs.Evaluaciones;
using ControlVehiculos.Models.Entities;
using ControlVehiculos.Repositories.Interfaces;
using ControlVehiculos.Services.Interfaces;

namespace ControlVehiculos.Services;

public class EvaluacionService : IEvaluacionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EvaluacionService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EvaluacionService(IUnitOfWork unitOfWork, ILogger<EvaluacionService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<EvaluacionDto?> RegisterAsync(RegisterEvaluacionRequest request)
    {
        // Obtener el inspector del usuario logueado
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var inspectorId))
        {
            throw new BusinessRuleException("unauthorized", "No se pudo identificar al usuario logueado");
        }

        // Validate inspector exists
        var inspector = await _unitOfWork.Usuarios.GetByIdAsync(inspectorId);
        if (inspector == null)
        {
            throw new NotFoundException("Inspector", inspectorId);
        }

        // Validate turno usando Repository
        var turno = await _unitOfWork.Turnos.GetByIdAsync(request.TurnoId);

        if (turno == null)
        {
            throw new NotFoundException("Turno", request.TurnoId);
        }

        // Check if already has evaluation
        var evaluacionExistente = await _unitOfWork.Evaluaciones.GetByTurnoIdWithIncludesAsync(request.TurnoId);
        
        if (evaluacionExistente != null)
        {
            // Solo permitir re-evaluación si el resultado anterior fue RECHEQUEO o CONDICIONAL
            if (evaluacionExistente.Resultado.Codigo == "SEGURO")
            {
                throw new ConflictException("duplicate_evaluation", "El turno ya tiene una evaluación con resultado SEGURO y no puede ser re-evaluado");
            }
            
            // Si hay una evaluación previa con RECHEQUEO o CONDICIONAL, eliminarla para permitir la nueva
            _unitOfWork.Evaluaciones.Delete(evaluacionExistente);
        }

        if (request.Detalles.Count != 8)
        {
            throw new BusinessRuleException("invalid_details_count", "Debe proporcionar exactamente 8 detalles de evaluación");
        }

        var puntajeTotal = request.Detalles.Sum(d => d.Puntaje);
        var resultadoCodigo = DetermineResultado(puntajeTotal, request.Detalles.Select(d => d.Puntaje).ToList());

        var resultado = await _unitOfWork.ResultadosEvaluacion.GetByCodigoAsync(resultadoCodigo);

        if (resultado == null)
        {
            throw new BusinessRuleException("catalog_error", $"ResultadoEvaluacion '{resultadoCodigo}' no encontrado en catálogos");
        }

        var evaluacion = new Evaluacion
        {
            Id = Guid.NewGuid(),
            TurnoId = request.TurnoId,
            InspectorId = inspectorId,
            Fecha = DateTime.UtcNow,
            PuntajeTotal = puntajeTotal,
            ResultadoEvaluacionId = resultado.Id
        };

        // Agregar detalles a la evaluación (EF Core los persistirá automáticamente)
        foreach (var detalleInput in request.Detalles)
        {
            evaluacion.Detalles.Add(new EvaluacionDetalle
            {
                Id = Guid.NewGuid(),
                EvaluacionId = evaluacion.Id,
                ChequeoId = detalleInput.ChequeoId,
                Puntaje = detalleInput.Puntaje,
                Observacion = detalleInput.Observacion
            });
        }

        await _unitOfWork.Evaluaciones.AddAsync(evaluacion);

        // Update turno status based on evaluation result
        // COMPLETADO: solo si el resultado es SEGURO (evaluación exitosa y final)
        // CONFIRMADO: si necesita re-evaluación (RECHEQUEO o CONDICIONAL)
        string estadoTurnoCodigo = resultadoCodigo == "SEGURO" ? "COMPLETADO" : "CONFIRMADO";
        var estadoTurno = await _unitOfWork.EstadosTurno.GetByCodigoAsync(estadoTurnoCodigo);
        if (estadoTurno != null)
        {
            turno.EstadoTurnoId = estadoTurno.Id;
            turno.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Turnos.Update(turno);
        }

        // Update vehicle status based on evaluation result
        var vehiculo = await _unitOfWork.Vehiculos.GetByIdAsync(turno.VehiculoId);
        if (vehiculo != null)
        {
            var estadoVehiculoCodigo = resultadoCodigo == "SEGURO" ? "SEGURO" : 
                                        resultadoCodigo == "RECHEQUEO" ? "RECHEQUEO" : 
                                        resultadoCodigo == "CONDICIONAL" ? "CONDICIONAL" : "PENDIENTE";

            var estadoVehiculo = await _unitOfWork.EstadosVehiculo.GetByCodigoAsync(estadoVehiculoCodigo);

            if (estadoVehiculo != null)
            {
                vehiculo.EstadoVehiculoId = estadoVehiculo.Id;
                _unitOfWork.Vehiculos.Update(vehiculo);
            }
        }

        await _unitOfWork.SaveChangesAsync();

        return await GetByIdAsync(evaluacion.Id);
    }

    public async Task<EvaluacionDto?> GetByIdAsync(Guid evaluacionId)
    {
        var evaluacion = await _unitOfWork.Evaluaciones.GetByIdWithIncludesAsync(evaluacionId);
        return evaluacion == null ? null : MapToDto(evaluacion);
    }

    private static string DetermineResultado(int puntajeTotal, List<int> puntajes)
    {
        if (puntajes.Any(p => p < 5))
        {
            return "RECHEQUEO";
        }

        if (puntajeTotal > 80)
        {
            return "SEGURO";
        }

        if (puntajeTotal < 40)
        {
            return "RECHEQUEO";
        }

        return "CONDICIONAL";
    }

    private static EvaluacionDto MapToDto(Evaluacion evaluacion)
    {
        return new EvaluacionDto
        {
            Id = evaluacion.Id,
            TurnoId = evaluacion.TurnoId,
            InspectorId = evaluacion.InspectorId,
            Fecha = evaluacion.Fecha,
            PuntajeTotal = evaluacion.PuntajeTotal,
            ResultadoEvaluacionId = evaluacion.ResultadoEvaluacionId,
            Resultado = new ResultadoEvaluacionDto
            {
                Id = evaluacion.Resultado.Id,
                Codigo = evaluacion.Resultado.Codigo,
                Nombre = evaluacion.Resultado.Nombre,
                Orden = evaluacion.Resultado.Orden
            },
            Detalles = evaluacion.Detalles.Select(d => new EvaluacionDetalleDto
            {
                Id = d.Id,
                EvaluacionId = d.EvaluacionId,
                ChequeoId = d.ChequeoId,
                Puntaje = d.Puntaje,
                Observacion = d.Observacion,
                Chequeo = new ChequeoDto
                {
                    Id = d.Chequeo.Id,
                    Nombre = d.Chequeo.Nombre,
                    Descripcion = d.Chequeo.Descripcion,
                    Orden = d.Chequeo.Orden
                }
            }).ToList()
        };
    }
}
