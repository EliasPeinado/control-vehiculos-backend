namespace ControlVehiculos.Models.Entities;

public class Vehiculo
{
    public Guid Id { get; set; }
    public string Matricula { get; set; } = string.Empty;
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public int? Anio { get; set; }
    public Guid PropietarioId { get; set; }
    public Guid EstadoVehiculoId { get; set; }
    
    // Navigation properties
    public Propietario Propietario { get; set; } = null!;
    public EstadoVehiculo EstadoVehiculo { get; set; } = null!;
    public ICollection<Turno> Turnos { get; set; } = new List<Turno>();
}
