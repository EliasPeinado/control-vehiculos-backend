using System.Text.Json;
using ControlVehiculos.Models.DTOs;

namespace ControlVehiculos.Middleware;

/// <summary>
/// Middleware para personalizar las respuestas de errores de autenticación y autorización
/// </summary>
public class AuthorizationResponseMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthorizationResponseMiddleware> _logger;

    public AuthorizationResponseMiddleware(RequestDelegate next, ILogger<AuthorizationResponseMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        // Personalizar respuestas 401 y 403
        if (context.Response.StatusCode == 401 && !context.Response.HasStarted)
        {
            await HandleUnauthorizedAsync(context);
        }
        else if (context.Response.StatusCode == 403 && !context.Response.HasStarted)
        {
            await HandleForbiddenAsync(context);
        }
    }

    private async Task HandleUnauthorizedAsync(HttpContext context)
    {
        _logger.LogWarning(
            "401 Unauthorized: {Method} {Path} desde {RemoteIp}",
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress);

        context.Response.ContentType = "application/json";
        
        var errorResponse = new ErrorResponse
        {
            Code = "unauthorized",
            Message = "Token de autenticación inválido o ausente. Por favor, inicie sesión.",
            TraceId = context.TraceIdentifier
        };

        var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    private async Task HandleForbiddenAsync(HttpContext context)
    {
        var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
        var roles = context.User?.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList() ?? new List<string>();

        _logger.LogWarning(
            "403 Forbidden: Usuario {UserId} con roles [{Roles}] intentó acceder a {Method} {Path}",
            userId,
            string.Join(", ", roles),
            context.Request.Method,
            context.Request.Path);

        context.Response.ContentType = "application/json";
        
        var errorResponse = new ErrorResponse
        {
            Code = "forbidden",
            Message = "No tiene permisos para realizar esta acción. Se requieren permisos adicionales.",
            TraceId = context.TraceIdentifier
        };

        var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
