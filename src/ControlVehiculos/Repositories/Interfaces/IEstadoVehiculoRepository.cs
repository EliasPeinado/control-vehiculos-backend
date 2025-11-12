using ControlVehiculos.Models.Entities;

namespace ControlVehiculos.Repositories.Interfaces;

public interface IEstadoVehiculoRepository : IRepository<EstadoVehiculo>
{
    Task<EstadoVehiculo?> GetByCodigoAsync(string codigo);
}
