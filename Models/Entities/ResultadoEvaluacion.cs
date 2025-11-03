namespace ControlVehiculos.Models.Entities;

public class ResultadoEvaluacion
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Orden { get; set; }
    
    // Navigation properties
    public ICollection<Evaluacion> Evaluaciones { get; set; } = new List<Evaluacion>();
}
