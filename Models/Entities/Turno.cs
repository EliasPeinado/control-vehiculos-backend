namespace ControlVehiculos.Models.Entities;

public class Turno
{
    public Guid Id { get; set; }
    public Guid VehiculoId { get; set; }
    public Guid CentroId { get; set; }
    public DateTime FechaHora { get; set; }
    public Guid EstadoTurnoId { get; set; }
    public string? MotivoCancelacion { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public Vehiculo Vehiculo { get; set; } = null!;
    public CentroInspeccion Centro { get; set; } = null!;
    public EstadoTurno EstadoTurno { get; set; } = null!;
    public Evaluacion? Evaluacion { get; set; }
}
