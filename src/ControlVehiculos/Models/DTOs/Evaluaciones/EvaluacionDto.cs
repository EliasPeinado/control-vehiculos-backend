namespace ControlVehiculos.Models.DTOs.Evaluaciones;

public class EvaluacionDto
{
    public Guid Id { get; set; }
    public Guid TurnoId { get; set; }
    public Guid InspectorId { get; set; }
    public DateTime Fecha { get; set; }
    public int PuntajeTotal { get; set; }
    public Guid ResultadoEvaluacionId { get; set; }
    public ResultadoEvaluacionDto Resultado { get; set; } = null!;
    public List<EvaluacionDetalleDto> Detalles { get; set; } = new();
}

public class ResultadoEvaluacionDto
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Orden { get; set; }
}

public class EvaluacionDetalleDto
{
    public Guid Id { get; set; }
    public Guid EvaluacionId { get; set; }
    public Guid ChequeoId { get; set; }
    public int Puntaje { get; set; }
    public string? Observacion { get; set; }
    public ChequeoDto Chequeo { get; set; } = null!;
}

public class ChequeoDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int Orden { get; set; }
}
