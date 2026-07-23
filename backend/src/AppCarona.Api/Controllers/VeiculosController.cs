using AppCarona.Contracts.Veiculos;
using AppCarona.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppCarona.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VeiculosController : AppControllerBase
{
    private readonly IVeiculoService _veiculoService;

    public VeiculosController(IVeiculoService veiculoService)
    {
        _veiculoService = veiculoService;
    }

    [HttpGet]
    public async Task<ActionResult<IList<VeiculoDto>>> Listar()
    {
        return Ok(await _veiculoService.ListarDoMotoristaAsync(UsuarioLogadoId));
    }

    [HttpPost]
    public async Task<ActionResult<VeiculoDto>> Criar([FromBody] SalvarVeiculoRequest request)
    {
        return Ok(await _veiculoService.CriarAsync(UsuarioLogadoId, request));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<VeiculoDto>> Atualizar(int id, [FromBody] SalvarVeiculoRequest request)
    {
        var veiculo = await _veiculoService.AtualizarAsync(UsuarioLogadoId, id, request);
        return veiculo is null ? NotFound() : Ok(veiculo);
    }
}
