using ControlVehiculos.Exceptions;
using ControlVehiculos.Models.DTOs.Turnos;
using ControlVehiculos.Models.Entities;
using ControlVehiculos.Repositories.Interfaces;
using ControlVehiculos.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace ControlVehiculos.Tests.Services;

public class TurnoServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<TurnoService>> _loggerMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly TurnoService _turnoService;

    public TurnoServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<TurnoService>>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        _turnoService = new TurnoService(
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithNewVehicle_CreatesVehicleAndTurno()
    {
        // Arrange
        var matricula = "ABC123";
        var centroId = Guid.NewGuid();
        var fechaHora = DateTime.UtcNow.AddDays(1);
        var usuarioId = Guid.NewGuid();

        var request = new CreateTurnoRequest
        {
            Matricula = matricula,
            CentroId = centroId,
            FechaHora = fechaHora
        };

        var estadoPendiente = new EstadoVehiculo
        {
            Id = Guid.NewGuid(),
            Codigo = "PENDIENTE",
            Nombre = "Pendiente",
            Orden = 1
        };

        var estadoReservado = new EstadoTurno
        {
            Id = Guid.NewGuid(),
            Codigo = "RESERVADO",
            Nombre = "Reservado",
            Orden = 1
        };

        var centro = new CentroInspeccion
        {
            Id = centroId,
            Nombre = "Centro Test",
            Direccion = "Dirección Test"
        };

        _unitOfWorkMock.Setup(x => x.Vehiculos.GetByMatriculaWithIncludesAsync(matricula))
            .ReturnsAsync((Vehiculo?)null);

        _unitOfWorkMock.Setup(x => x.EstadosVehiculo.GetByCodigoAsync("PENDIENTE"))
            .ReturnsAsync(estadoPendiente);

        _unitOfWorkMock.Setup(x => x.Turnos.ExistsSlotAsync(centroId, fechaHora))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(x => x.EstadosTurno.GetByCodigoAsync("RESERVADO"))
            .ReturnsAsync(estadoReservado);

        _unitOfWorkMock.Setup(x => x.Centros.GetByIdAsync(centroId))
            .ReturnsAsync(centro);

        _unitOfWorkMock.Setup(x => x.Propietarios.AddAsync(It.IsAny<Propietario>()))
            .ReturnsAsync(It.IsAny<Propietario>());

        _unitOfWorkMock.Setup(x => x.Vehiculos.AddAsync(It.IsAny<Vehiculo>()))
            .ReturnsAsync(It.IsAny<Vehiculo>());

        _unitOfWorkMock.Setup(x => x.Turnos.AddAsync(It.IsAny<Turno>()))
            .ReturnsAsync(It.IsAny<Turno>());

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        var turnoCreado = new Turno
        {
            Id = Guid.NewGuid(),
            VehiculoId = Guid.NewGuid(),
            CentroId = centroId,
            FechaHora = fechaHora,
            EstadoTurnoId = estadoReservado.Id,
            CreatedAt = DateTime.UtcNow,
            Vehiculo = new Vehiculo
            {
                Id = Guid.NewGuid(),
                Matricula = matricula.ToUpperInvariant(),
                PropietarioId = Guid.NewGuid(),
                EstadoVehiculoId = estadoPendiente.Id,
                Propietario = new Propietario
                {
                    Id = Guid.NewGuid(),
                    Nombre = "Propietario Pendiente"
                },
                EstadoVehiculo = estadoPendiente
            },
            Centro = centro,
            EstadoTurno = estadoReservado
        };

        _unitOfWorkMock.Setup(x => x.Turnos.GetByIdWithIncludesAsync(It.IsAny<Guid>()))
            .ReturnsAsync(turnoCreado);

        // Mock HttpContext para usuario autenticado
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = await _turnoService.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Vehiculo.Matricula.Should().Be(matricula.ToUpperInvariant());
        result.Centro.Id.Should().Be(centroId);
        result.FechaHora.Should().Be(fechaHora);

        _unitOfWorkMock.Verify(x => x.Propietarios.AddAsync(It.IsAny<Propietario>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Vehiculos.AddAsync(It.IsAny<Vehiculo>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Turnos.AddAsync(It.IsAny<Turno>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithOccupiedSlot_ThrowsConflictException()
    {
        // Arrange
        var matricula = "ABC123";
        var centroId = Guid.NewGuid();
        var fechaHora = DateTime.UtcNow.AddDays(1);

        var request = new CreateTurnoRequest
        {
            Matricula = matricula,
            CentroId = centroId,
            FechaHora = fechaHora
        };

        var vehiculo = new Vehiculo
        {
            Id = Guid.NewGuid(),
            Matricula = matricula,
            PropietarioId = Guid.NewGuid(),
            EstadoVehiculoId = Guid.NewGuid()
        };

        _unitOfWorkMock.Setup(x => x.Vehiculos.GetByMatriculaWithIncludesAsync(matricula))
            .ReturnsAsync(vehiculo);

        _unitOfWorkMock.Setup(x => x.Turnos.ExistsSlotAsync(centroId, fechaHora))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _turnoService.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Ya existe un turno para (centro, fechaHora)");
    }

    [Fact]
    public async Task ConfirmarAsync_WithReservedTurno_UpdatesToConfirmado()
    {
        // Arrange
        var turnoId = Guid.NewGuid();
        var estadoReservadoId = Guid.NewGuid();
        var estadoConfirmadoId = Guid.NewGuid();

        var turno = new Turno
        {
            Id = turnoId,
            VehiculoId = Guid.NewGuid(),
            CentroId = Guid.NewGuid(),
            FechaHora = DateTime.UtcNow.AddDays(1),
            EstadoTurnoId = estadoReservadoId,
            CreatedAt = DateTime.UtcNow
        };

        var estadoReservado = new EstadoTurno
        {
            Id = estadoReservadoId,
            Codigo = "RESERVADO",
            Nombre = "Reservado",
            Orden = 1
        };

        var estadoConfirmado = new EstadoTurno
        {
            Id = estadoConfirmadoId,
            Codigo = "CONFIRMADO",
            Nombre = "Confirmado",
            Orden = 2
        };

        _unitOfWorkMock.Setup(x => x.Turnos.GetByIdAsync(turnoId))
            .ReturnsAsync(turno);

        _unitOfWorkMock.Setup(x => x.EstadosTurno.GetByIdAsync(estadoReservadoId))
            .ReturnsAsync(estadoReservado);

        _unitOfWorkMock.Setup(x => x.EstadosTurno.GetByCodigoAsync("CONFIRMADO"))
            .ReturnsAsync(estadoConfirmado);

        // Act
        var result = await _turnoService.ConfirmarAsync(turnoId);

        // Assert
        result.Should().BeTrue();
        turno.EstadoTurnoId.Should().Be(estadoConfirmadoId);
        turno.UpdatedAt.Should().NotBeNull();

        _unitOfWorkMock.Verify(x => x.Turnos.Update(turno), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ConfirmarAsync_WithNonReservedTurno_ThrowsBusinessRuleException()
    {
        // Arrange
        var turnoId = Guid.NewGuid();
        var estadoCompletadoId = Guid.NewGuid();

        var turno = new Turno
        {
            Id = turnoId,
            VehiculoId = Guid.NewGuid(),
            CentroId = Guid.NewGuid(),
            FechaHora = DateTime.UtcNow.AddDays(1),
            EstadoTurnoId = estadoCompletadoId,
            CreatedAt = DateTime.UtcNow
        };

        var estadoCompletado = new EstadoTurno
        {
            Id = estadoCompletadoId,
            Codigo = "COMPLETADO",
            Nombre = "Completado",
            Orden = 3
        };

        _unitOfWorkMock.Setup(x => x.Turnos.GetByIdAsync(turnoId))
            .ReturnsAsync(turno);

        _unitOfWorkMock.Setup(x => x.EstadosTurno.GetByIdAsync(estadoCompletadoId))
            .ReturnsAsync(estadoCompletado);

        // Act
        Func<Task> act = async () => await _turnoService.ConfirmarAsync(turnoId);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("Solo se pueden confirmar turnos en estado RESERVADO");
    }

    [Fact]
    public async Task GetFilteredAsync_ReturnsFilteredTurnos()
    {
        // Arrange
        var centroId = Guid.NewGuid();
        var matricula = "ABC123";

        var turnos = new List<Turno>
        {
            new Turno
            {
                Id = Guid.NewGuid(),
                VehiculoId = Guid.NewGuid(),
                CentroId = centroId,
                FechaHora = DateTime.UtcNow.AddDays(1),
                EstadoTurnoId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Vehiculo = new Vehiculo
                {
                    Id = Guid.NewGuid(),
                    Matricula = matricula,
                    PropietarioId = Guid.NewGuid(),
                    EstadoVehiculoId = Guid.NewGuid(),
                    Propietario = new Propietario
                    {
                        Id = Guid.NewGuid(),
                        Nombre = "Test Owner"
                    },
                    EstadoVehiculo = new EstadoVehiculo
                    {
                        Id = Guid.NewGuid(),
                        Codigo = "PENDIENTE",
                        Nombre = "Pendiente",
                        Orden = 1
                    }
                },
                Centro = new CentroInspeccion
                {
                    Id = centroId,
                    Nombre = "Centro Test",
                    Direccion = "Dirección Test"
                },
                EstadoTurno = new EstadoTurno
                {
                    Id = Guid.NewGuid(),
                    Codigo = "RESERVADO",
                    Nombre = "Reservado",
                    Orden = 1
                }
            }
        };

        _unitOfWorkMock.Setup(x => x.Turnos.GetFilteredAsync(centroId, matricula))
            .ReturnsAsync(turnos);

        // Act
        var result = await _turnoService.GetFilteredAsync(centroId, matricula);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].Vehiculo.Matricula.Should().Be(matricula);
        result[0].Centro.Id.Should().Be(centroId);
    }
}
