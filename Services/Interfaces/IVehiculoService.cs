using ControlVehiculos.Models.DTOs.Vehiculos;
using ControlVehiculos.Models.DTOs.Evaluaciones;
using ControlVehiculos.Models.DTOs;
using ControlVehiculos.Models.DTOs.Turnos;

namespace ControlVehiculos.Services.Interfaces;

public interface IVehiculoService
{
    Task<VehiculoDto> CreateAsync(CreateVehiculoRequest request);
    Task<VehiculoDto> GetByMatriculaAsync(string matricula);
    Task<VehiculoDto> UpdateAsync(string matricula, UpdateVehiculoRequest request);
    Task<PagedResponse<EvaluacionDto>> GetEvaluacionesByMatriculaAsync(string matricula, int page, int pageSize);
    Task<List<TurnoDto>> GetTurnosByMatriculaAsync(string matricula);
    Task<bool> ExistsByMatriculaAsync(string matricula);
}
