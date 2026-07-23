using AppCarona.Contracts.Motorista;
using AppCarona.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppCarona.Api.Controllers;

[ApiController]
[Route("api/motorista/config")]
[Authorize]
public class MotoristaConfigController : AppControllerBase
{
    private readonly IMotoristaConfigService _configService;

    public MotoristaConfigController(IMotoristaConfigService configService)
    {
        _configService = configService;
    }

    [HttpGet]
    public async Task<ActionResult<MotoristaConfigDto>> Obter()
    {
        var config = await _configService.ObterAsync(UsuarioLogadoId);
        return config is null ? NoContent() : Ok(config);
    }

    [HttpPut]
    public async Task<ActionResult<MotoristaConfigDto>> Salvar([FromBody] SalvarMotoristaConfigRequest request)
    {
        return Ok(await _configService.SalvarAsync(UsuarioLogadoId, request));
    }
}
