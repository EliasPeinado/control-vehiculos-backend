using Microsoft.EntityFrameworkCore;
using ControlVehiculos.Data;
using ControlVehiculos.Models.Entities;
using ControlVehiculos.Repositories.Interfaces;

namespace ControlVehiculos.Repositories;

// Repositorios simples que solo heredan de Repository<T>
public class PropietarioRepository : Repository<Propietario>, IPropietarioRepository
{
    public PropietarioRepository(ApplicationDbContext context) : base(context) { }
}

public class CentroInspeccionRepository : Repository<CentroInspeccion>, ICentroInspeccionRepository
{
    public CentroInspeccionRepository(ApplicationDbContext context) : base(context) { }
}

public class RolRepository : Repository<Rol>, IRolRepository
{
    public RolRepository(ApplicationDbContext context) : base(context) { }
}

// Repositorios con métodos personalizados
public class EstadoTurnoRepository : Repository<EstadoTurno>, IEstadoTurnoRepository
{
    public EstadoTurnoRepository(ApplicationDbContext context) : base(context) { }

    public async Task<EstadoTurno?> GetByCodigoAsync(string codigo)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Codigo == codigo);
    }
}

public class EstadoVehiculoRepository : Repository<EstadoVehiculo>, IEstadoVehiculoRepository
{
    public EstadoVehiculoRepository(ApplicationDbContext context) : base(context) { }

    public async Task<EstadoVehiculo?> GetByCodigoAsync(string codigo)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Codigo == codigo);
    }
}

public class ResultadoEvaluacionRepository : Repository<ResultadoEvaluacion>, IResultadoEvaluacionRepository
{
    public ResultadoEvaluacionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<ResultadoEvaluacion?> GetByCodigoAsync(string codigo)
    {
        return await _dbSet.FirstOrDefaultAsync(r => r.Codigo == codigo);
    }
}

public class ChequeoRepository : Repository<Chequeo>, IChequeoRepository
{
    public ChequeoRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Chequeo>> GetActivosOrdenadosAsync()
    {
        return await _dbSet
            .Where(c => c.Activo)
            .OrderBy(c => c.Orden)
            .ToListAsync();
    }
}

public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Usuario?> GetByEmailWithRolesAsync(string email)
    {
        return await _dbSet
            .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<(IEnumerable<Usuario> Items, int Total)> GetAllWithRolesPagedAsync(int page, int pageSize)
    {
        var query = _dbSet
            .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
            .OrderBy(u => u.Nombre);

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}

public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(ApplicationDbContext context) : base(context) { }

    public async Task<RefreshToken?> GetByTokenWithUserAsync(string token)
    {
        return await _dbSet
            .Include(rt => rt.Usuario)
                .ThenInclude(u => u.UsuarioRoles)
                    .ThenInclude(ur => ur.Rol)
            .FirstOrDefaultAsync(rt => rt.Token == token && !rt.Revoked);
    }
}
