namespace ControlVehiculos.Models.Entities;

public class EstadoVehiculo
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Orden { get; set; }
    
    // Navigation properties
    public ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
