using AppCarona.Contracts.Motorista;
using AppCarona.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppCarona.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MotoristasController : ControllerBase
{
    private readonly IMotoristaService _motoristaService;

    public MotoristasController(IMotoristaService motoristaService)
    {
        _motoristaService = motoristaService;
    }

    [HttpGet]
    public async Task<ActionResult<IList<MotoristaOpcaoDto>>> Listar()
    {
        return Ok(await _motoristaService.ListarOpcoesAsync());
    }

    [HttpGet("{id:int}/detalhes")]
    public async Task<ActionResult<DetalheMotoristaDto>> Detalhes(int id, [FromQuery] int anoMes)
    {
        var detalhes = await _motoristaService.DetalhesAsync(id, anoMes);
        return detalhes is null ? NotFound() : Ok(detalhes);
    }
}
