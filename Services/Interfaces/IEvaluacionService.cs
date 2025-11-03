using ControlVehiculos.Models.DTOs.Evaluaciones;

namespace ControlVehiculos.Services.Interfaces;

public interface IEvaluacionService
{
    Task<EvaluacionDto?> RegisterAsync(RegisterEvaluacionRequest request);
    Task<EvaluacionDto?> GetByIdAsync(Guid evaluacionId);
}
