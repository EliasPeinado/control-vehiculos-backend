namespace ControlVehiculos.Models.DTOs.Turnos;

public class SlotDto
{
    public DateTime Inicio { get; set; }
    public DateTime Fin { get; set; }
    public bool Disponible { get; set; }
}
