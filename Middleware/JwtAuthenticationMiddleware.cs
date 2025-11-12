using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ControlVehiculos.Middleware;

/// <summary>
/// Middleware personalizado para autenticación JWT con logging detallado
/// Este middleware se ejecuta DESPUÉS de UseAuthentication() built-in
/// para agregar logging y validaciones adicionales
/// </summary>
public class JwtAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtAuthenticationMiddleware> _logger;

    public JwtAuthenticationMiddleware(
        RequestDelegate next, 
        ILogger<JwtAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Obtener el token del header Authorization
        var token = GetTokenFromHeader(context);

        if (!string.IsNullOrEmpty(token))
        {
            // Log del intento de autenticación
            _logger.LogInformation(
                "Token JWT detectado en request {Method} {Path}", 
                context.Request.Method, 
                context.Request.Path);

            // Validar el token (ya fue validado por UseAuthentication, pero podemos agregar checks adicionales)
            ValidateToken(context, token);
        }
        else if (RequiresAuthentication(context))
        {
            // Endpoint requiere autenticación pero no hay token
            _logger.LogWarning(
                "Intento de acceso sin token a endpoint protegido: {Method} {Path} desde {RemoteIp}",
                context.Request.Method,
                context.Request.Path,
                context.Connection.RemoteIpAddress);
        }

        await _next(context);

        // Log del resultado de autenticación
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                         ?? context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var email = context.User.FindFirst(ClaimTypes.Email)?.Value 
                        ?? context.User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
            var roles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            _logger.LogInformation(
                "Usuario autenticado: {UserId} ({Email}) con roles [{Roles}]",
                userId,
                email,
                string.Join(", ", roles));
        }
        else if (context.Response.StatusCode == 401)
        {
            _logger.LogWarning(
                "Autenticación fallida para {Method} {Path} - Status: 401 Unauthorized",
                context.Request.Method,
                context.Request.Path);
        }
        else if (context.Response.StatusCode == 403)
        {
            _logger.LogWarning(
                "Autorización denegada para {Method} {Path} - Status: 403 Forbidden",
                context.Request.Method,
                context.Request.Path);
        }
    }

    private static string? GetTokenFromHeader(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(authHeader))
        {
            return null;
        }

        // El header debe tener el formato: "Bearer {token}"
        if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }

        return null;
    }

    private void ValidateToken(HttpContext context, string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Log información del token
            _logger.LogDebug(
                "Token JWT: Issuer={Issuer}, Subject={Subject}, Expires={Expires}",
                jwtToken.Issuer,
                jwtToken.Subject,
                jwtToken.ValidTo);

            // Validar expiración (redundante pero útil para logging)
            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                _logger.LogWarning(
                    "Token JWT expirado. Expiró: {ExpirationTime}, Ahora: {CurrentTime}",
                    jwtToken.ValidTo,
                    DateTime.UtcNow);
            }

            // Validar claims requeridos
            var hasSub = jwtToken.Claims.Any(c => c.Type == JwtRegisteredClaimNames.Sub);
            var hasEmail = jwtToken.Claims.Any(c => c.Type == JwtRegisteredClaimNames.Email);

            if (!hasSub || !hasEmail)
            {
                _logger.LogWarning(
                    "Token JWT incompleto. HasSub={HasSub}, HasEmail={HasEmail}",
                    hasSub,
                    hasEmail);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar token JWT");
        }
    }

    private static bool RequiresAuthentication(HttpContext context)
    {
        // Obtener el endpoint
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            return false;
        }

        // Verificar si tiene el atributo [Authorize]
        var metadata = endpoint.Metadata;
        var requiresAuth = metadata.GetMetadata<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>() != null;
        var allowsAnonymous = metadata.GetMetadata<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>() != null;

        return requiresAuth && !allowsAnonymous;
    }
}
