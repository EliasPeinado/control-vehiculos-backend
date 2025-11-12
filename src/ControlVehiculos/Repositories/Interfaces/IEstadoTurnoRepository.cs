using ControlVehiculos.Models.Entities;

namespace ControlVehiculos.Repositories.Interfaces;

public interface IEstadoTurnoRepository : IRepository<EstadoTurno>
{
    Task<EstadoTurno?> GetByCodigoAsync(string codigo);
}
