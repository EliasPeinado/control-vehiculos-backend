using Microsoft.EntityFrameworkCore;
using ControlVehiculos.Data;
using ControlVehiculos.Models.Entities;
using ControlVehiculos.Repositories.Interfaces;

namespace ControlVehiculos.Repositories;

public class VehiculoRepository : Repository<Vehiculo>, IVehiculoRepository
{
    public VehiculoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Vehiculo?> GetByMatriculaWithIncludesAsync(string matricula)
    {
        return await _dbSet
            .Include(v => v.Propietario)
            .Include(v => v.EstadoVehiculo)
            .FirstOrDefaultAsync(v => v.Matricula == matricula);
    }

    public async Task<bool> ExistsByMatriculaAsync(string matricula)
    {
        return await _dbSet.AnyAsync(v => v.Matricula == matricula);
    }
}
