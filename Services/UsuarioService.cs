using ControlVehiculos.Models.DTOs;
using ControlVehiculos.Models.DTOs.Usuarios;
using ControlVehiculos.Repositories.Interfaces;
using ControlVehiculos.Services.Interfaces;

namespace ControlVehiculos.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUnitOfWork _unitOfWork;

    public UsuarioService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResponse<UsuarioDto>> GetAllAsync(int page, int pageSize)
    {
        var (items, total) = await _unitOfWork.Usuarios.GetAllWithRolesPagedAsync(page, pageSize);

        return new PagedResponse<UsuarioDto>
        {
            Meta = new PageMeta
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                HasNext = page * pageSize < total
            },
            Items = items.Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Email = u.Email,
                Roles = u.UsuarioRoles.Select(ur => new RolDto
                {
                    Id = ur.Rol.Id,
                    Codigo = ur.Rol.Codigo,
                    Nombre = ur.Rol.Nombre
                }).ToList()
            }).ToList()
        };
    }
}
