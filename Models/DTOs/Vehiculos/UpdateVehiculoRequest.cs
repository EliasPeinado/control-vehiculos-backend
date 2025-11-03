namespace ControlVehiculos.Models.DTOs.Vehiculos;

public class UpdateVehiculoRequest
{
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public int? Anio { get; set; }
    public Guid? PropietarioId { get; set; }
    public Guid? EstadoVehiculoId { get; set; }
}
