using ControlVehiculos.Models.Entities;

namespace ControlVehiculos.Repositories.Interfaces;

public interface IResultadoEvaluacionRepository : IRepository<ResultadoEvaluacion>
{
    Task<ResultadoEvaluacion?> GetByCodigoAsync(string codigo);
}
