using ControlVehiculos.Models.DTOs.Auth;
using ControlVehiculos.Models.Entities;
using ControlVehiculos.Repositories.Interfaces;
using ControlVehiculos.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControlVehiculos.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _configurationMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<AuthService>>();

        // Configurar JWT settings mock
        var jwtSection = new Mock<IConfigurationSection>();
        jwtSection.Setup(x => x["SecretKey"]).Returns("TestSecretKey_MuyLargaYSegura_MinimoDe32Caracteres!");
        jwtSection.Setup(x => x["Issuer"]).Returns("TestIssuer");
        jwtSection.Setup(x => x["Audience"]).Returns("TestAudience");
        _configurationMock.Setup(x => x.GetSection("JwtSettings")).Returns(jwtSection.Object);

        _authService = new AuthService(_unitOfWorkMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Email = email,
            Nombre = "Test User",
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        var rol = new Rol
        {
            Id = Guid.NewGuid(),
            Codigo = "CLIENTE",
            Nombre = "Cliente"
        };

        var usuarioRol = new UsuarioRol
        {
            UsuarioId = usuario.Id,
            RolId = rol.Id,
            Rol = rol
        };

        usuario.UsuarioRoles = new List<UsuarioRol> { usuarioRol };

        _unitOfWorkMock.Setup(x => x.Usuarios.GetByEmailWithRolesAsync(email))
            .ReturnsAsync(usuario);
        
        var refreshTokensMock = new Mock<IRefreshTokenRepository>();
        _unitOfWorkMock.Setup(x => x.RefreshTokens).Returns(refreshTokensMock.Object);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = password
        };

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.TokenType.Should().Be("Bearer");
        result.CodigoRol.Should().Be("CLIENTE");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ReturnsNull()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var password = "Password123!";

        _unitOfWorkMock.Setup(x => x.Usuarios.GetByEmailWithRolesAsync(email))
            .ReturnsAsync((Usuario?)null);

        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = password
        };

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ReturnsNull()
    {
        // Arrange
        var email = "test@example.com";
        var correctPassword = "Password123!";
        var wrongPassword = "WrongPassword!";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(correctPassword);

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Email = email,
            Nombre = "Test User",
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        _unitOfWorkMock.Setup(x => x.Usuarios.GetByEmailWithRolesAsync(email))
            .ReturnsAsync(usuario);

        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = wrongPassword
        };

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GenerateAccessToken_WithValidData_ReturnsToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var roles = new List<string> { "CLIENTE" };

        // Act
        var token = _authService.GenerateAccessToken(userId, email, roles);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3); // JWT tiene 3 partes separadas por punto
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsUniqueToken()
    {
        // Act
        var token1 = _authService.GenerateRefreshToken();
        var token2 = _authService.GenerateRefreshToken();

        // Assert
        token1.Should().NotBeNullOrEmpty();
        token2.Should().NotBeNullOrEmpty();
        token1.Should().NotBe(token2); // Cada token debe ser único
    }
}
