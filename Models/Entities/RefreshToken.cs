namespace ControlVehiculos.Models.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool Revoked { get; set; }
    
    // Navigation properties
    public Usuario Usuario { get; set; } = null!;
}
