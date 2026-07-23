using AppCarona.Contracts.Pagamentos;
using AppCarona.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppCarona.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PagamentosController : AppControllerBase
{
    private readonly IPagamentoService _pagamentoService;

    public PagamentosController(IPagamentoService pagamentoService)
    {
        _pagamentoService = pagamentoService;
    }

    /// <summary>Passageiro: quanto devo a cada motorista no mês.</summary>
    [HttpGet("resumo")]
    public async Task<ActionResult<IList<PagamentoResumoDto>>> Resumo([FromQuery] int anoMes)
    {
        return Ok(await _pagamentoService.ResumoDoPassageiroAsync(UsuarioLogadoId, anoMes));
    }

    /// <summary>Passageiro: informar que pagou um motorista.</summary>
    [HttpPost("informar")]
    public async Task<IActionResult> Informar([FromBody] InformarPagamentoRequest request)
    {
        await _pagamentoService.InformarAsync(UsuarioLogadoId, request.MotoristaId, request.AnoMes);
        return NoContent();
    }

    /// <summary>Motorista: lista de pagadores do mês.</summary>
    [HttpGet("recebimentos")]
    public async Task<ActionResult<IList<RecebimentoDto>>> Recebimentos([FromQuery] int anoMes)
    {
        return Ok(await _pagamentoService.RecebimentosAsync(UsuarioLogadoId, anoMes));
    }

    /// <summary>Resumo do mês por motorista (todos): passageiros, dias e valores.</summary>
    [HttpGet("resumo-mensal")]
    public async Task<ActionResult<IList<ResumoMensalMotoristaDto>>> ResumoMensal([FromQuery] int anoMes)
    {
        return Ok(await _pagamentoService.ResumoMensalAsync(anoMes));
    }

    /// <summary>Motorista: confirmar recebimento.</summary>
    [HttpPut("{id:int}/confirmar")]
    public async Task<IActionResult> Confirmar(int id)
    {
        await _pagamentoService.ConfirmarAsync(UsuarioLogadoId, id);
        return NoContent();
    }

    /// <summary>Motorista: rejeitar pagamento informado.</summary>
    [HttpPut("{id:int}/rejeitar")]
    public async Task<IActionResult> Rejeitar(int id)
    {
        await _pagamentoService.RejeitarAsync(UsuarioLogadoId, id);
        return NoContent();
    }
}
