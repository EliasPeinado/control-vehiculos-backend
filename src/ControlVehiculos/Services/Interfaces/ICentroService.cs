using ControlVehiculos.Models.DTOs;
using ControlVehiculos.Models.DTOs.Turnos;

namespace ControlVehiculos.Services.Interfaces;

public interface ICentroService
{
    Task<PagedResponse<CentroInspeccionDto>> GetAllAsync(int page, int pageSize);
}
