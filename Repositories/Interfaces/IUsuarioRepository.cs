using ControlVehiculos.Models.Entities;

namespace ControlVehiculos.Repositories.Interfaces;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByEmailWithRolesAsync(string email);
    Task<(IEnumerable<Usuario> Items, int Total)> GetAllWithRolesPagedAsync(int page, int pageSize);
}
