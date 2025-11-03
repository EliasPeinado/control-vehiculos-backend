using System.Net;
using System.Text.Json;
using ControlVehiculos.Exceptions;
using ControlVehiculos.Models.DTOs;

namespace ControlVehiculos.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            TraceId = context.TraceIdentifier
        };

        switch (exception)
        {
            case NotFoundException notFoundEx:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Code = "not_found";
                errorResponse.Message = notFoundEx.Message;
                break;

            case BusinessRuleException businessEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Code = businessEx.Code;
                errorResponse.Message = businessEx.Message;
                break;

            case ConflictException conflictEx:
                response.StatusCode = (int)HttpStatusCode.Conflict;
                errorResponse.Code = conflictEx.Code;
                errorResponse.Message = conflictEx.Message;
                break;

            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Code = "unauthorized";
                errorResponse.Message = "No autorizado para realizar esta acción";
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Code = "internal_error";
                errorResponse.Message = "Ha ocurrido un error interno en el servidor";
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }
}
