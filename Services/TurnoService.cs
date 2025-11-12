using ControlVehiculos.Exceptions;
using ControlVehiculos.Models.DTOs.Turnos;
using ControlVehiculos.Models.DTOs.Vehiculos;
using ControlVehiculos.Models.Entities;
using ControlVehiculos.Repositories.Interfaces;
using ControlVehiculos.Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ControlVehiculos.Services;

public class TurnoService : ITurnoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TurnoService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TurnoService(IUnitOfWork unitOfWork, ILogger<TurnoService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<SlotDto>> GetSlotsAsync(Guid centroId, DateTime fecha)
    {
        var slots = new List<SlotDto>();
        var startTime = fecha.Date.AddHours(8);
        var endTime = fecha.Date.AddHours(18);

        // Usar Repository en lugar de _context
        var existingTurnos = await _unitOfWork.Turnos.GetFechasOcupadasAsync(centroId, fecha);

        for (var time = startTime; time < endTime; time = time.AddMinutes(30))
        {
            slots.Add(new SlotDto
            {
                Inicio = DateTime.SpecifyKind(time, DateTimeKind.Utc),
                Fin = DateTime.SpecifyKind(time.AddMinutes(30), DateTimeKind.Utc),
                Disponible = !existingTurnos.Any(t => t == time)
            });
        }

        return slots;
    }

    public async Task<TurnoDto?> CreateAsync(CreateTurnoRequest request)
    {
        // Check if vehiculo exists usando Repository
        var vehiculo = await _unitOfWork.Vehiculos.GetByMatriculaWithIncludesAsync(request.Matricula);

        if (vehiculo == null)
        {
            // Auto-create vehiculo
            var estadoPendiente = await _unitOfWork.EstadosVehiculo.GetByCodigoAsync("PENDIENTE");

            if (estadoPendiente == null)
            {
                throw new BusinessRuleException("catalog_error", "EstadoVehiculo 'PENDIENTE' no encontrado en catálogos");
            }

            var propietario = new Propietario
            {
                Id = Guid.NewGuid(),
                Nombre = "Propietario Pendiente"
            };

            vehiculo = new Vehiculo
            {
                Id = Guid.NewGuid(),
                Matricula = request.Matricula.ToUpperInvariant(),
                PropietarioId = propietario.Id,
                EstadoVehiculoId = estadoPendiente.Id,
                Propietario = propietario,
                EstadoVehiculo = estadoPendiente
            };

            await _unitOfWork.Propietarios.AddAsync(propietario);
            await _unitOfWork.Vehiculos.AddAsync(vehiculo);
        }

        // Check if slot is available usando Repository
        var existingTurno = await _unitOfWork.Turnos.ExistsSlotAsync(request.CentroId, request.FechaHora);

        if (existingTurno)
        {
            throw new ConflictException("slot_conflict", "Ya existe un turno para (centro, fechaHora)");
        }

        // Get EstadoTurno "RESERVADO" usando Repository
        var estadoReservado = await _unitOfWork.EstadosTurno.GetByCodigoAsync("RESERVADO");

        if (estadoReservado == null)
        {
            throw new BusinessRuleException("catalog_error", "EstadoTurno 'RESERVADO' no encontrado en catálogos");
        }

        var centro = await _unitOfWork.Centros.GetByIdAsync(request.CentroId);
        if (centro == null)
        {
            throw new NotFoundException("Centro", request.CentroId);
        }

        // Obtener el usuario logueado del contexto
        Guid? creadoPorUsuarioId = null;
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
        {
            creadoPorUsuarioId = userId;
        }

        var turno = new Turno
        {
            Id = Guid.NewGuid(),
            VehiculoId = vehiculo.Id,
            CentroId = request.CentroId,
            FechaHora = request.FechaHora,
            EstadoTurnoId = estadoReservado.Id,
            CreadoPorUsuarioId = creadoPorUsuarioId,
            CreatedAt = DateTime.UtcNow,
            Vehiculo = vehiculo,
            Centro = centro,
            EstadoTurno = estadoReservado
        };

        await _unitOfWork.Turnos.AddAsync(turno);
        await _unitOfWork.SaveChangesAsync();

        // Recargar con el usuario incluido
        var turnoCompleto = await _unitOfWork.Turnos.GetByIdWithIncludesAsync(turno.Id);
        return MapToDto(turnoCompleto!);
    }

    public async Task<TurnoDto?> GetByIdAsync(Guid turnoId)
    {
        var turno = await _unitOfWork.Turnos.GetByIdWithIncludesAsync(turnoId);
        return turno == null ? null : MapToDto(turno);
    }

    public async Task<bool> ConfirmarAsync(Guid turnoId)
    {
        var turno = await _unitOfWork.Turnos.GetByIdAsync(turnoId);

        if (turno == null)
        {
            return false;
        }

        // Load EstadoTurno para validar
        var estadoTurno = await _unitOfWork.EstadosTurno.GetByIdAsync(turno.EstadoTurnoId);
        
        if (estadoTurno == null || estadoTurno.Codigo != "RESERVADO")
        {
            throw new BusinessRuleException("invalid_state", "Solo se pueden confirmar turnos en estado RESERVADO");
        }

        var estadoConfirmado = await _unitOfWork.EstadosTurno.GetByCodigoAsync("CONFIRMADO");

        if (estadoConfirmado == null)
        {
            throw new BusinessRuleException("catalog_error", "EstadoTurno 'CONFIRMADO' no encontrado en catálogos");
        }

        turno.EstadoTurnoId = estadoConfirmado.Id;
        turno.UpdatedAt = DateTime.UtcNow;
        
        _unitOfWork.Turnos.Update(turno);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CancelarAsync(Guid turnoId, string motivo)
    {
        var turno = await _unitOfWork.Turnos.GetByIdAsync(turnoId);

        if (turno == null)
        {
            return false;
        }

        var estadoCancelado = await _unitOfWork.EstadosTurno.GetByCodigoAsync("CANCELADO");

        if (estadoCancelado == null)
        {
            throw new BusinessRuleException("catalog_error", "EstadoTurno 'CANCELADO' no encontrado en catálogos");
        }

        turno.EstadoTurnoId = estadoCancelado.Id;
        turno.MotivoCancelacion = motivo;
        turno.UpdatedAt = DateTime.UtcNow;
        
        _unitOfWork.Turnos.Update(turno);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<List<TurnoDto>> GetFilteredAsync(Guid? centroId = null, string? matricula = null)
    {
        var turnos = await _unitOfWork.Turnos.GetFilteredAsync(centroId, matricula);
        return turnos.Select(MapToDto).ToList();
    }

    private static TurnoDto MapToDto(Turno turno)
    {
        return new TurnoDto
        {
            Id = turno.Id,
            VehiculoId = turno.VehiculoId,
            CentroId = turno.CentroId,
            FechaHora = turno.FechaHora,
            EstadoTurnoId = turno.EstadoTurnoId,
            CreadoPorUsuarioId = turno.CreadoPorUsuarioId,
            EstadoTurno = new EstadoTurnoDto
            {
                Id = turno.EstadoTurno.Id,
                Codigo = turno.EstadoTurno.Codigo,
                Nombre = turno.EstadoTurno.Nombre,
                Orden = turno.EstadoTurno.Orden
            },
            Vehiculo = new VehiculoDto
            {
                Id = turno.Vehiculo.Id,
                Matricula = turno.Vehiculo.Matricula,
                Marca = turno.Vehiculo.Marca,
                Modelo = turno.Vehiculo.Modelo,
                Anio = turno.Vehiculo.Anio,
                PropietarioId = turno.Vehiculo.PropietarioId,
                EstadoVehiculoId = turno.Vehiculo.EstadoVehiculoId,
                Propietario = new PropietarioDto
                {
                    Id = turno.Vehiculo.Propietario.Id,
                    Nombre = turno.Vehiculo.Propietario.Nombre,
                    Email = turno.Vehiculo.Propietario.Email,
                    Telefono = turno.Vehiculo.Propietario.Telefono
                },
                EstadoVehiculo = new EstadoVehiculoDto
                {
                    Id = turno.Vehiculo.EstadoVehiculo.Id,
                    Codigo = turno.Vehiculo.EstadoVehiculo.Codigo,
                    Nombre = turno.Vehiculo.EstadoVehiculo.Nombre,
                    Orden = turno.Vehiculo.EstadoVehiculo.Orden
                }
            },
            Centro = new CentroInspeccionDto
            {
                Id = turno.Centro.Id,
                Nombre = turno.Centro.Nombre,
                Direccion = turno.Centro.Direccion
            },
            CreadoPorUsuario = turno.CreadoPorUsuario != null ? new UsuarioSimpleDto
            {
                Id = turno.CreadoPorUsuario.Id,
                Nombre = turno.CreadoPorUsuario.Nombre,
                Email = turno.CreadoPorUsuario.Email
            } : null
        };
    }
}
