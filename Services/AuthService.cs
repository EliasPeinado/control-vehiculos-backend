using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ControlVehiculos.Models.DTOs.Auth;
using ControlVehiculos.Models.Entities;
using ControlVehiculos.Repositories.Interfaces;
using ControlVehiculos.Services.Interfaces;

namespace ControlVehiculos.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var usuario = await _unitOfWork.Usuarios.GetByEmailWithRolesAsync(request.Email);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
        {
            return null;
        }

        var roles = usuario.UsuarioRoles.Select(ur => ur.Rol.Codigo).ToList();
        var accessToken = GenerateAccessToken(usuario.Id, usuario.Email, roles);
        var refreshToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            Revoked = false
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
        await _unitOfWork.SaveChangesAsync();

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            TokenType = "Bearer",
            ExpiresIn = 3600
        };
    }

    public async Task<LoginResponse?> RefreshTokenAsync(string refreshToken)
    {
        var tokenEntity = await _unitOfWork.RefreshTokens.GetByTokenWithUserAsync(refreshToken);

        if (tokenEntity == null || tokenEntity.ExpiresAt < DateTime.UtcNow)
        {
            return null;
        }

        var usuario = tokenEntity.Usuario;
        var roles = usuario.UsuarioRoles.Select(ur => ur.Rol.Codigo).ToList();
        var newAccessToken = GenerateAccessToken(usuario.Id, usuario.Email, roles);
        var newRefreshToken = GenerateRefreshToken();

        tokenEntity.Revoked = true;
        _unitOfWork.RefreshTokens.Update(tokenEntity);

        var newRefreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            Revoked = false
        };

        await _unitOfWork.RefreshTokens.AddAsync(newRefreshTokenEntity);
        await _unitOfWork.SaveChangesAsync();

        return new LoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            TokenType = "Bearer",
            ExpiresIn = 3600
        };
    }

    public string GenerateAccessToken(Guid userId, string email, List<string> roles)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
