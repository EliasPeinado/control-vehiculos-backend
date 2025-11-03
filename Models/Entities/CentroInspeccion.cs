namespace ControlVehiculos.Models.Entities;

public class CentroInspeccion
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    
    // Navigation properties
    public ICollection<Turno> Turnos { get; set; } = new List<Turno>();
}
