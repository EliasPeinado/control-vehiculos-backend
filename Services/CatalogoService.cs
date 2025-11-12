using ControlVehiculos.Models.DTOs.Evaluaciones;
using ControlVehiculos.Models.DTOs.Turnos;
using ControlVehiculos.Models.DTOs.Vehiculos;
using ControlVehiculos.Repositories.Interfaces;
using ControlVehiculos.Services.Interfaces;

namespace ControlVehiculos.Services;

public class CatalogoService : ICatalogoService
{
    private readonly IUnitOfWork _unitOfWork;

    public CatalogoService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ChequeoDto>> GetChequeosAsync()
    {
        var chequeos = await _unitOfWork.Chequeos.GetActivosOrdenadosAsync();

        return chequeos.Select(c => new ChequeoDto
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Descripcion = c.Descripcion,
            Orden = c.Orden
        }).ToList();
    }

    public async Task<List<EstadoTurnoDto>> GetEstadosTurnoAsync()
    {
        var estados = (await _unitOfWork.EstadosTurno.GetAllAsync()).OrderBy(e => e.Orden);

        return estados.Select(e => new EstadoTurnoDto
        {
            Id = e.Id,
            Codigo = e.Codigo,
            Nombre = e.Nombre,
            Orden = e.Orden
        }).ToList();
    }

    public async Task<List<EstadoVehiculoDto>> GetEstadosVehiculoAsync()
    {
        var estados = (await _unitOfWork.EstadosVehiculo.GetAllAsync()).OrderBy(e => e.Orden);

        return estados.Select(e => new EstadoVehiculoDto
        {
            Id = e.Id,
            Codigo = e.Codigo,
            Nombre = e.Nombre,
            Orden = e.Orden
        }).ToList();
    }

    public async Task<List<ResultadoEvaluacionDto>> GetResultadosEvaluacionAsync()
    {
        var resultados = (await _unitOfWork.ResultadosEvaluacion.GetAllAsync()).OrderBy(r => r.Orden);

        return resultados.Select(r => new ResultadoEvaluacionDto
        {
            Id = r.Id,
            Codigo = r.Codigo,
            Nombre = r.Nombre,
            Orden = r.Orden
        }).ToList();
    }
}
