namespace ControlVehiculos.Models.DTOs;

public class ErrorResponse
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? TraceId { get; set; }
    public List<ErrorDetail>? Details { get; set; }
}

public class ErrorDetail
{
    public string Field { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
}
