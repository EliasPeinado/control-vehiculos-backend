using ControlVehiculos.Models.Entities;

namespace ControlVehiculos.Repositories.Interfaces;

public interface IVehiculoRepository : IRepository<Vehiculo>
{
    Task<Vehiculo?> GetByMatriculaWithIncludesAsync(string matricula);
    Task<bool> ExistsByMatriculaAsync(string matricula);
}
