namespace ControlVehiculos.Models.DTOs.Vehiculos;

public class VehiculoDto
{
    public Guid Id { get; set; }
    public string Matricula { get; set; } = string.Empty;
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public int? Anio { get; set; }
    public Guid PropietarioId { get; set; }
    public Guid EstadoVehiculoId { get; set; }
    public PropietarioDto Propietario { get; set; } = null!;
    public EstadoVehiculoDto EstadoVehiculo { get; set; } = null!;
}

public class PropietarioDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefono { get; set; }
}

public class EstadoVehiculoDto
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Orden { get; set; }
}
