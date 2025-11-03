using System.ComponentModel.DataAnnotations;

namespace ControlVehiculos.Models.DTOs.Vehiculos;

public class CreateVehiculoRequest
{
    [Required]
    [StringLength(12, MinimumLength = 5)]
    public string Matricula { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Marca { get; set; }

    [StringLength(100)]
    public string? Modelo { get; set; }

    [Range(1900, 2100)]
    public int? Anio { get; set; }

    public Guid? PropietarioId { get; set; }

    public Guid? EstadoVehiculoId { get; set; }
}
