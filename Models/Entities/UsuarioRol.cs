namespace ControlVehiculos.Models.Entities;

public class UsuarioRol
{
    public Guid UsuarioId { get; set; }
    public Guid RolId { get; set; }
    
    // Navigation properties
    public Usuario Usuario { get; set; } = null!;
    public Rol Rol { get; set; } = null!;
}
