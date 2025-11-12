using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControlVehiculos.Models.DTOs;
using ControlVehiculos.Services.Interfaces;

namespace ControlVehiculos.Controllers;

[ApiController]
[Route("v1/usuarios")]
[Authorize(Roles = "ADMIN")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var result = await _usuarioService.GetAllAsync(page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse
            {
                Code = "internal_error",
                Message = ex.Message,
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }
}
