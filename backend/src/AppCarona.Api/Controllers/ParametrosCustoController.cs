using AppCarona.Contracts.Custo;
using AppCarona.Domain.Enums;
using AppCarona.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppCarona.Api.Controllers;

[ApiController]
[Route("api/parametros-custo")]
[Authorize(Roles = nameof(Papel.Master))]
public class ParametrosCustoController : ControllerBase
{
    private readonly IParametroCustoService _parametroService;

    public ParametrosCustoController(IParametroCustoService parametroService)
    {
        _parametroService = parametroService;
    }

    [HttpGet]
    public async Task<ActionResult<IList<ParametroCustoDto>>> Listar()
    {
        return Ok(await _parametroService.ListarAsync());
    }

    [HttpPost]
    public async Task<ActionResult<ParametroCustoDto>> Salvar([FromBody] SalvarParametroCustoRequest request)
    {
        return Ok(await _parametroService.SalvarAsync(request));
    }
}
