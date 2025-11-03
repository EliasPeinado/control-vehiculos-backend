using Microsoft.EntityFrameworkCore;
using ControlVehiculos.Data;
using ControlVehiculos.Models.Entities;
using ControlVehiculos.Repositories.Interfaces;

namespace ControlVehiculos.Repositories;

public class TurnoRepository : Repository<Turno>, ITurnoRepository
{
    public TurnoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Turno?> GetByIdWithIncludesAsync(Guid id)
    {
        return await _dbSet
            .Include(t => t.Vehiculo)
                .ThenInclude(v => v.Propietario)
            .Include(t => t.Vehiculo.EstadoVehiculo)
            .Include(t => t.Centro)
            .Include(t => t.EstadoTurno)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<DateTime>> GetFechasOcupadasAsync(Guid centroId, DateTime fecha)
    {
        return await _dbSet
            .Where(t => t.CentroId == centroId && t.FechaHora.Date == fecha.Date)
            .Select(t => t.FechaHora)
            .ToListAsync();
    }

    public async Task<bool> ExistsSlotAsync(Guid centroId, DateTime fechaHora)
    {
        return await _dbSet.AnyAsync(t => t.CentroId == centroId && t.FechaHora == fechaHora);
    }
}
