namespace ControlVehiculos.Models.Entities;

public class Chequeo
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int Orden { get; set; }
    public bool Activo { get; set; } = true;
    
    // Navigation properties
    public ICollection<EvaluacionDetalle> EvaluacionDetalles { get; set; } = new List<EvaluacionDetalle>();
}
