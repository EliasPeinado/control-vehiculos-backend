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
            .Include(t => t.CreadoPorUsuario)
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

    public async Task<List<Turno>> GetByMatriculaAsync(string matricula)
    {
        return await _dbSet
            .Include(t => t.Vehiculo)
                .ThenInclude(v => v.Propietario)
            .Include(t => t.Vehiculo.EstadoVehiculo)
            .Include(t => t.Centro)
            .Include(t => t.EstadoTurno)
            .Include(t => t.CreadoPorUsuario)
            .Where(t => t.Vehiculo.Matricula == matricula)
            .OrderByDescending(t => t.FechaHora)
            .ToListAsync();
    }

    public async Task<List<Turno>> GetFilteredAsync(Guid? centroId = null, string? matricula = null)
    {
        var query = _dbSet
            .Include(t => t.Vehiculo)
                .ThenInclude(v => v.Propietario)
            .Include(t => t.Vehiculo.EstadoVehiculo)
            .Include(t => t.Centro)
            .Include(t => t.EstadoTurno)
            .Include(t => t.CreadoPorUsuario)
            .AsQueryable();

        if (centroId.HasValue)
        {
            query = query.Where(t => t.CentroId == centroId.Value);
        }

        if (!string.IsNullOrWhiteSpace(matricula))
        {
            var matriculaLower = matricula.ToLower();
            query = query.Where(t => t.Vehiculo.Matricula.ToLower().Contains(matriculaLower));
        }

        // Filtrar turnos:
        // - Estado RESERVADO: sin evaluación (pendientes)
        // - Estado CONFIRMADO: con evaluación RECHEQUEO o CONDICIONAL (necesitan re-evaluación)
        // - Excluir estado COMPLETADO: evaluación SEGURO (ya finalizados)
        // - Excluir estado CANCELADO: turnos cancelados
        query = query.Where(t => 
            t.EstadoTurno.Codigo == "RESERVADO" || 
            t.EstadoTurno.Codigo == "CONFIRMADO"
        );

        return await query
            .OrderByDescending(t => t.FechaHora)
            .ToListAsync();
    }
}
