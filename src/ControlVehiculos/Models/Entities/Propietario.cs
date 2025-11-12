namespace ControlVehiculos.Models.Entities;

public class Propietario
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    
    // Navigation properties
    public ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
