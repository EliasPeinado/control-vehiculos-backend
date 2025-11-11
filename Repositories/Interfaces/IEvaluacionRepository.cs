using ControlVehiculos.Models.Entities;

namespace ControlVehiculos.Repositories.Interfaces;

public interface IEvaluacionRepository : IRepository<Evaluacion>
{
    Task<Evaluacion?> GetByIdWithIncludesAsync(Guid id);
    Task<Evaluacion?> GetByTurnoIdWithIncludesAsync(Guid turnoId);
    Task<(IEnumerable<Evaluacion> Items, int Total)> GetByVehiculoIdPagedAsync(Guid vehiculoId, int page, int pageSize);
}
