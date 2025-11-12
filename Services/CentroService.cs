using ControlVehiculos.Models.DTOs;
using ControlVehiculos.Models.DTOs.Turnos;
using ControlVehiculos.Repositories.Interfaces;
using ControlVehiculos.Services.Interfaces;

namespace ControlVehiculos.Services;

public class CentroService : ICentroService
{
    private readonly IUnitOfWork _unitOfWork;

    public CentroService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResponse<CentroInspeccionDto>> GetAllAsync(int page, int pageSize)
    {
        var (items, total) = await _unitOfWork.Centros.GetPagedAsync(
            page, 
            pageSize,
            orderBy: c => c.Nombre,
            ascending: true);

        return new PagedResponse<CentroInspeccionDto>
        {
            Meta = new PageMeta
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                HasNext = page * pageSize < total
            },
            Items = items.Select(c => new CentroInspeccionDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Direccion = c.Direccion
            }).ToList()
        };
    }
}
