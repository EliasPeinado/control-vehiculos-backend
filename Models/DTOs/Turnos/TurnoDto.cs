using ControlVehiculos.Models.DTOs.Vehiculos;

namespace ControlVehiculos.Models.DTOs.Turnos;

public class TurnoDto
{
    public Guid Id { get; set; }
    public Guid VehiculoId { get; set; }
    public Guid CentroId { get; set; }
    public DateTime FechaHora { get; set; }
    public Guid EstadoTurnoId { get; set; }
    public EstadoTurnoDto EstadoTurno { get; set; } = null!;
    public VehiculoDto Vehiculo { get; set; } = null!;
    public CentroInspeccionDto Centro { get; set; } = null!;
}

public class EstadoTurnoDto
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Orden { get; set; }
}

public class CentroInspeccionDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Direccion { get; set; }
}
