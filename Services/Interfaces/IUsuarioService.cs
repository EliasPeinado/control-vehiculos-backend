using ControlVehiculos.Models.DTOs;
using ControlVehiculos.Models.DTOs.Usuarios;

namespace ControlVehiculos.Services.Interfaces;

public interface IUsuarioService
{
    Task<PagedResponse<UsuarioDto>> GetAllAsync(int page, int pageSize);
}
