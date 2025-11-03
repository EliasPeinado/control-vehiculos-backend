namespace ControlVehiculos.Models.Entities;

public class EstadoTurno
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Orden { get; set; }
    
    // Navigation properties
    public ICollection<Turno> Turnos { get; set; } = new List<Turno>();
}
