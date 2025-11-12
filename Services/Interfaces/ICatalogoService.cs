using ControlVehiculos.Models.DTOs.Evaluaciones;
using ControlVehiculos.Models.DTOs.Turnos;
using ControlVehiculos.Models.DTOs.Vehiculos;

namespace ControlVehiculos.Services.Interfaces;

public interface ICatalogoService
{
    Task<List<ChequeoDto>> GetChequeosAsync();
    Task<List<EstadoTurnoDto>> GetEstadosTurnoAsync();
    Task<List<EstadoVehiculoDto>> GetEstadosVehiculoAsync();
    Task<List<ResultadoEvaluacionDto>> GetResultadosEvaluacionAsync();
}
