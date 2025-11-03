using System.Text.Json;
using ControlVehiculos.Models.DTOs;

namespace ControlVehiculos.Middleware;

public class RequestValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestValidationMiddleware> _logger;

    public RequestValidationMiddleware(RequestDelegate next, ILogger<RequestValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log request
        _logger.LogInformation(
            "Request: {Method} {Path} from {RemoteIp}",
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress);

        await _next(context);

        // Log response status
        _logger.LogInformation(
            "Response: {StatusCode} for {Method} {Path}",
            context.Response.StatusCode,
            context.Request.Method,
            context.Request.Path);
    }
}
