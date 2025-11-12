namespace ControlVehiculos.Models.Entities;

public class EvaluacionDetalle
{
    public Guid Id { get; set; }
    public Guid EvaluacionId { get; set; }
    public Guid ChequeoId { get; set; }
    public int Puntaje { get; set; }
    public string? Observacion { get; set; }
    
    // Navigation properties
    public Evaluacion Evaluacion { get; set; } = null!;
    public Chequeo Chequeo { get; set; } = null!;
}
