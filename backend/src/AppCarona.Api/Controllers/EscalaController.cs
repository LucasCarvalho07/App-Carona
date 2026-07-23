using AppCarona.Contracts.Escala;
using AppCarona.Domain.Enums;
using AppCarona.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppCarona.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EscalaController : AppControllerBase
{
    private readonly IEscalaService _escalaService;

    public EscalaController(IEscalaService escalaService)
    {
        _escalaService = escalaService;
    }

    [HttpGet]
    public async Task<ActionResult<IList<EscalaCarroDto>>> Listar([FromQuery] int anoMes)
    {
        return Ok(await _escalaService.ListarEscalaMesAsync(UsuarioLogadoId, anoMes));
    }

    [HttpPost]
    public async Task<IActionResult> Escalar([FromBody] EscalarRequest request)
    {
        var sentido = Enum.Parse<Sentido>(request.Sentido, ignoreCase: true);
        await _escalaService.EscalarAsync(UsuarioLogadoId, request.Data, sentido);
        return NoContent();
    }

    [HttpPost("semana")]
    public async Task<IActionResult> EscalarSemana([FromBody] EscalarSemanaRequest request)
    {
        await _escalaService.EscalarSemanaAsync(UsuarioLogadoId, request.DataNaSemana);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Desescalar([FromBody] EscalarRequest request)
    {
        var sentido = Enum.Parse<Sentido>(request.Sentido, ignoreCase: true);
        await _escalaService.DesescalarAsync(UsuarioLogadoId, request.Data, sentido);
        return NoContent();
    }
}
