using System.ComponentModel.DataAnnotations;

namespace ControlVehiculos.Models.DTOs.Turnos;

public class CreateTurnoRequest
{
    [Required]
    public string Matricula { get; set; } = string.Empty;
    
    [Required]
    public Guid CentroId { get; set; }
    
    [Required]
    public DateTime FechaHora { get; set; }
}
