using System.ComponentModel.DataAnnotations;

namespace ControlVehiculos.Models.DTOs.Turnos;

public class CancelarTurnoRequest
{
    [Required]
    [MaxLength(200)]
    public string Motivo { get; set; } = string.Empty;
}
