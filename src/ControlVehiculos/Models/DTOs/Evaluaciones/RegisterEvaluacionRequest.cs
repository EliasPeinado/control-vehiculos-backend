using System.ComponentModel.DataAnnotations;

namespace ControlVehiculos.Models.DTOs.Evaluaciones;

public class RegisterEvaluacionRequest
{
    [Required]
    public Guid TurnoId { get; set; }
    
    [Required]
    [MinLength(8)]
    [MaxLength(8)]
    public List<EvaluacionDetalleInput> Detalles { get; set; } = new();
}

public class EvaluacionDetalleInput
{
    [Required]
    public Guid ChequeoId { get; set; }
    
    [Required]
    [Range(1, 10)]
    public int Puntaje { get; set; }
    
    [MaxLength(500)]
    public string? Observacion { get; set; }
}
