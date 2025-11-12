using ControlVehiculos.Models.Entities;

namespace ControlVehiculos.Repositories.Interfaces;

public interface IChequeoRepository : IRepository<Chequeo>
{
    Task<IEnumerable<Chequeo>> GetActivosOrdenadosAsync();
}
