using ControlVehiculos.Models.DTOs.Turnos;

namespace ControlVehiculos.Services.Interfaces;

public interface ITurnoService
{
    Task<List<SlotDto>> GetSlotsAsync(Guid centroId, DateTime fecha);
    Task<TurnoDto?> CreateAsync(CreateTurnoRequest request);
    Task<TurnoDto?> GetByIdAsync(Guid turnoId);
    Task<bool> ConfirmarAsync(Guid turnoId);
    Task<bool> CancelarAsync(Guid turnoId, string motivo);
    Task<List<TurnoDto>> GetFilteredAsync(Guid? centroId = null, string? matricula = null);
}
