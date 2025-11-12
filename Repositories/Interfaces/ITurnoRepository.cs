using ControlVehiculos.Models.Entities;

namespace ControlVehiculos.Repositories.Interfaces;

public interface ITurnoRepository : IRepository<Turno>
{
    Task<Turno?> GetByIdWithIncludesAsync(Guid id);
    Task<IEnumerable<DateTime>> GetFechasOcupadasAsync(Guid centroId, DateTime fecha);
    Task<bool> ExistsSlotAsync(Guid centroId, DateTime fechaHora);
    Task<List<Turno>> GetByMatriculaAsync(string matricula);
    Task<List<Turno>> GetFilteredAsync(Guid? centroId = null, string? matricula = null);
}
