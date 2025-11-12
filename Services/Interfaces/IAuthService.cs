using ControlVehiculos.Models.DTOs.Auth;

namespace ControlVehiculos.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<LoginResponse?> RefreshTokenAsync(string refreshToken);
    string GenerateAccessToken(Guid userId, string email, List<string> roles);
    string GenerateRefreshToken();
}
