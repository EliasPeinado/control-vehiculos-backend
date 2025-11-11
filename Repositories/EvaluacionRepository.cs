using Microsoft.EntityFrameworkCore;
using ControlVehiculos.Data;
using ControlVehiculos.Models.Entities;
using ControlVehiculos.Repositories.Interfaces;

namespace ControlVehiculos.Repositories;

public class EvaluacionRepository : Repository<Evaluacion>, IEvaluacionRepository
{
    public EvaluacionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Evaluacion?> GetByIdWithIncludesAsync(Guid id)
    {
        return await _dbSet
            .Include(e => e.Resultado)
            .Include(e => e.Detalles)
                .ThenInclude(d => d.Chequeo)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Evaluacion?> GetByTurnoIdWithIncludesAsync(Guid turnoId)
    {
        return await _dbSet
            .Include(e => e.Resultado)
            .Include(e => e.Detalles)
                .ThenInclude(d => d.Chequeo)
            .FirstOrDefaultAsync(e => e.TurnoId == turnoId);
    }

    public async Task<(IEnumerable<Evaluacion> Items, int Total)> GetByVehiculoIdPagedAsync(Guid vehiculoId, int page, int pageSize)
    {
        var query = _dbSet
            .Include(e => e.Resultado)
            .Include(e => e.Detalles)
                .ThenInclude(d => d.Chequeo)
            .Include(e => e.Turno)
            .Where(e => e.Turno.VehiculoId == vehiculoId)
            .OrderByDescending(e => e.Fecha);

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}
