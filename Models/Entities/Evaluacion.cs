namespace ControlVehiculos.Models.Entities;

public class Evaluacion
{
    public Guid Id { get; set; }
    public Guid TurnoId { get; set; }
    public Guid InspectorId { get; set; }
    public DateTime Fecha { get; set; }
    public int PuntajeTotal { get; set; }
    public Guid ResultadoEvaluacionId { get; set; }
    
    // Navigation properties
    public Turno Turno { get; set; } = null!;
    public Usuario Inspector { get; set; } = null!;
    public ResultadoEvaluacion Resultado { get; set; } = null!;
    public ICollection<EvaluacionDetalle> Detalles { get; set; } = new List<EvaluacionDetalle>();
}
