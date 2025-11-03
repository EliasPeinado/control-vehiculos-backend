using ControlVehiculos.Exceptions;
using ControlVehiculos.Models.DTOs;
using ControlVehiculos.Models.DTOs.Evaluaciones;
using ControlVehiculos.Models.DTOs.Vehiculos;
using ControlVehiculos.Models.Entities;
using ControlVehiculos.Repositories.Interfaces;
using ControlVehiculos.Services.Interfaces;

namespace ControlVehiculos.Services;

/// <summary>
/// Service refactorizado usando el patrón Repository
/// YA NO interactúa directamente con DbContext
/// Usa IUnitOfWork para acceder a los repositorios
/// </summary>
public class VehiculoService : IVehiculoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VehiculoService> _logger;

    public VehiculoService(IUnitOfWork unitOfWork, ILogger<VehiculoService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<VehiculoDto> CreateAsync(CreateVehiculoRequest request)
    {
        // Validar unicidad usando Repository
        var exists = await _unitOfWork.Vehiculos.ExistsByMatriculaAsync(request.Matricula);
        if (exists)
        {
            throw new ConflictException("duplicate_matricula", 
                $"Ya existe un vehículo con la matrícula {request.Matricula}");
        }

        // Get or validate EstadoVehiculo usando Repository
        Guid estadoVehiculoId;
        if (request.EstadoVehiculoId.HasValue)
        {
            estadoVehiculoId = request.EstadoVehiculoId.Value;
            var estadoExists = await _unitOfWork.EstadosVehiculo.AnyAsync(ev => ev.Id == estadoVehiculoId);
            if (!estadoExists)
            {
                throw new NotFoundException("EstadoVehiculo", estadoVehiculoId);
            }
        }
        else
        {
            var estadoPendiente = await _unitOfWork.EstadosVehiculo.GetByCodigoAsync("PENDIENTE");
            if (estadoPendiente == null)
            {
                throw new BusinessRuleException("catalog_error", 
                    "EstadoVehiculo 'PENDIENTE' no encontrado en catálogos");
            }
            estadoVehiculoId = estadoPendiente.Id;
        }

        // Get or create Propietario usando Repository
        Guid propietarioId;
        if (request.PropietarioId.HasValue)
        {
            propietarioId = request.PropietarioId.Value;
            var propietarioExists = await _unitOfWork.Propietarios.AnyAsync(p => p.Id == propietarioId);
            if (!propietarioExists)
            {
                throw new NotFoundException("Propietario", propietarioId);
            }
        }
        else
        {
            var propietario = new Propietario
            {
                Id = Guid.NewGuid(),
                Nombre = "Propietario Pendiente"
            };
            await _unitOfWork.Propietarios.AddAsync(propietario);
            propietarioId = propietario.Id;
        }

        // Crear vehículo
        var vehiculo = new Vehiculo
        {
            Id = Guid.NewGuid(),
            Matricula = request.Matricula.ToUpperInvariant(),
            Marca = request.Marca,
            Modelo = request.Modelo,
            Anio = request.Anio,
            PropietarioId = propietarioId,
            EstadoVehiculoId = estadoVehiculoId
        };

        // Agregar usando Repository
        await _unitOfWork.Vehiculos.AddAsync(vehiculo);
        
        // IMPORTANTE: SaveChanges a través del UnitOfWork
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Vehículo creado: {Matricula} con ID {Id}", 
            vehiculo.Matricula, vehiculo.Id);

        // Cargar relaciones usando Repository
        var vehiculoCompleto = await _unitOfWork.Vehiculos.GetByMatriculaWithIncludesAsync(vehiculo.Matricula);
        
        return MapToDto(vehiculoCompleto!);
    }

    public async Task<VehiculoDto> GetByMatriculaAsync(string matricula)
    {
        // Usar método específico del Repository que ya incluye las relaciones
        var vehiculo = await _unitOfWork.Vehiculos.GetByMatriculaWithIncludesAsync(matricula);

        if (vehiculo == null)
        {
            throw new NotFoundException("Vehiculo", matricula);
        }

        return MapToDto(vehiculo);
    }

    public async Task<VehiculoDto> UpdateAsync(string matricula, UpdateVehiculoRequest request)
    {
        // Obtener vehículo usando Repository
        var vehiculo = await _unitOfWork.Vehiculos.GetByMatriculaWithIncludesAsync(matricula);

        if (vehiculo == null)
        {
            throw new NotFoundException("Vehiculo", matricula);
        }

        // Validar foreign keys usando Repository
        if (request.PropietarioId.HasValue)
        {
            var propietarioExists = await _unitOfWork.Propietarios.AnyAsync(p => p.Id == request.PropietarioId.Value);
            if (!propietarioExists)
            {
                throw new NotFoundException("Propietario", request.PropietarioId.Value);
            }
            vehiculo.PropietarioId = request.PropietarioId.Value;
        }

        if (request.EstadoVehiculoId.HasValue)
        {
            var estadoExists = await _unitOfWork.EstadosVehiculo.AnyAsync(ev => ev.Id == request.EstadoVehiculoId.Value);
            if (!estadoExists)
            {
                throw new NotFoundException("EstadoVehiculo", request.EstadoVehiculoId.Value);
            }
            vehiculo.EstadoVehiculoId = request.EstadoVehiculoId.Value;
        }

        // Actualizar propiedades simples
        if (!string.IsNullOrWhiteSpace(request.Marca))
        {
            vehiculo.Marca = request.Marca;
        }

        if (!string.IsNullOrWhiteSpace(request.Modelo))
        {
            vehiculo.Modelo = request.Modelo;
        }

        if (request.Anio.HasValue)
        {
            vehiculo.Anio = request.Anio.Value;
        }

        // Actualizar usando Repository
        _unitOfWork.Vehiculos.Update(vehiculo);
        
        // SaveChanges a través del UnitOfWork
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Vehículo actualizado: {Matricula}", matricula);

        // Recargar con relaciones
        var vehiculoActualizado = await _unitOfWork.Vehiculos.GetByMatriculaWithIncludesAsync(matricula);
        
        return MapToDto(vehiculoActualizado!);
    }

    public async Task<PagedResponse<EvaluacionDto>> GetEvaluacionesByMatriculaAsync(string matricula, int page, int pageSize)
    {
        // Obtener vehículo usando Repository
        var vehiculo = await _unitOfWork.Vehiculos.FirstOrDefaultAsync(v => v.Matricula == matricula);

        if (vehiculo == null)
        {
            throw new NotFoundException("Vehiculo", matricula);
        }

        // Obtener evaluaciones usando Repository
        var (items, total) = await _unitOfWork.Evaluaciones.GetByVehiculoIdPagedAsync(vehiculo.Id, page, pageSize);

        return new PagedResponse<EvaluacionDto>
        {
            Meta = new PageMeta
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                HasNext = page * pageSize < total
            },
            Items = items.Select(MapEvaluacionToDto).ToList()
        };
    }

    public async Task<bool> ExistsByMatriculaAsync(string matricula)
    {
        // Usar Repository
        return await _unitOfWork.Vehiculos.ExistsByMatriculaAsync(matricula);
    }

    // Métodos de mapeo (sin cambios)
    private static VehiculoDto MapToDto(Vehiculo vehiculo)
    {
        return new VehiculoDto
        {
            Id = vehiculo.Id,
            Matricula = vehiculo.Matricula,
            Marca = vehiculo.Marca,
            Modelo = vehiculo.Modelo,
            Anio = vehiculo.Anio,
            PropietarioId = vehiculo.PropietarioId,
            EstadoVehiculoId = vehiculo.EstadoVehiculoId,
            Propietario = new PropietarioDto
            {
                Id = vehiculo.Propietario.Id,
                Nombre = vehiculo.Propietario.Nombre,
                Email = vehiculo.Propietario.Email,
                Telefono = vehiculo.Propietario.Telefono
            },
            EstadoVehiculo = new EstadoVehiculoDto
            {
                Id = vehiculo.EstadoVehiculo.Id,
                Codigo = vehiculo.EstadoVehiculo.Codigo,
                Nombre = vehiculo.EstadoVehiculo.Nombre,
                Orden = vehiculo.EstadoVehiculo.Orden
            }
        };
    }

    private static EvaluacionDto MapEvaluacionToDto(Evaluacion evaluacion)
    {
        return new EvaluacionDto
        {
            Id = evaluacion.Id,
            TurnoId = evaluacion.TurnoId,
            InspectorId = evaluacion.InspectorId,
            Fecha = evaluacion.Fecha,
            PuntajeTotal = evaluacion.PuntajeTotal,
            ResultadoEvaluacionId = evaluacion.ResultadoEvaluacionId,
            Resultado = new ResultadoEvaluacionDto
            {
                Id = evaluacion.Resultado.Id,
                Codigo = evaluacion.Resultado.Codigo,
                Nombre = evaluacion.Resultado.Nombre,
                Orden = evaluacion.Resultado.Orden
            },
            Detalles = evaluacion.Detalles.Select(d => new EvaluacionDetalleDto
            {
                Id = d.Id,
                EvaluacionId = d.EvaluacionId,
                ChequeoId = d.ChequeoId,
                Puntaje = d.Puntaje,
                Observacion = d.Observacion,
                Chequeo = new ChequeoDto
                {
                    Id = d.Chequeo.Id,
                    Nombre = d.Chequeo.Nombre,
                    Descripcion = d.Chequeo.Descripcion,
                    Orden = d.Chequeo.Orden
                }
            }).ToList()
        };
    }
}
