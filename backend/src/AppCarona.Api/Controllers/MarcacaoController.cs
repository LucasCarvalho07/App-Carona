using AppCarona.Contracts.Marcacao;
using AppCarona.Domain.Enums;
using AppCarona.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppCarona.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MarcacaoController : AppControllerBase
{
    private readonly IMarcacaoService _marcacaoService;

    public MarcacaoController(IMarcacaoService marcacaoService)
    {
        _marcacaoService = marcacaoService;
    }

    [HttpGet]
    public async Task<ActionResult<IList<MinhaMarcacaoDto>>> Minhas([FromQuery] int anoMes)
    {
        return Ok(await _marcacaoService.ListarMinhasAsync(UsuarioLogadoId, anoMes));
    }

    [HttpPost]
    public async Task<ActionResult<MinhaMarcacaoDto>> Marcar([FromBody] MarcarPresencaRequest request)
    {
        var sentido = Enum.Parse<Sentido>(request.Sentido, ignoreCase: true);
        return Ok(await _marcacaoService.MarcarAsync(UsuarioLogadoId, request.Data, request.MotoristaId, sentido));
    }

    [HttpDelete]
    public async Task<IActionResult> Desmarcar([FromBody] MarcarPresencaRequest request)
    {
        var sentido = Enum.Parse<Sentido>(request.Sentido, ignoreCase: true);
        await _marcacaoService.DesmarcarAsync(UsuarioLogadoId, request.Data, request.MotoristaId, sentido);
        return NoContent();
    }
}
