using Microsoft.AspNetCore.Mvc;
using ControlVehiculos.Models.DTOs;
using ControlVehiculos.Models.DTOs.Auth;
using ControlVehiculos.Services.Interfaces;

namespace ControlVehiculos.Controllers;

[ApiController]
[Route("v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);

        if (response == null)
        {
            return Unauthorized(new ErrorResponse
            {
                Code = "unauthorized",
                Message = "Credenciales inválidas",
                TraceId = HttpContext.TraceIdentifier
            });
        }

        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var response = await _authService.RefreshTokenAsync(request.RefreshToken);

        if (response == null)
        {
            return Unauthorized(new ErrorResponse
            {
                Code = "unauthorized",
                Message = "Refresh token inválido o expirado",
                TraceId = HttpContext.TraceIdentifier
            });
        }

        return Ok(response);
    }
}
