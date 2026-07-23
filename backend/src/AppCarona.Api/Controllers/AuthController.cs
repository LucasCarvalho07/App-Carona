using AppCarona.Contracts.Auth;
using AppCarona.Domain.Enums;
using AppCarona.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AppCarona.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("google")]
    public async Task<ActionResult<AuthResponse>> LoginGoogle([FromBody] LoginGoogleRequest request)
    {
        return Ok(await _authService.LoginComGoogleAsync(request.IdToken));
    }

    [HttpPost("registrar")]
    public async Task<ActionResult<AuthResponse>> Registrar([FromBody] RegistrarRequest request)
    {
        return Ok(await _authService.RegistrarLocalAsync(
            request.Nome, request.Email, request.Telefone, request.Senha));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginLocalRequest request)
    {
        return Ok(await _authService.LoginLocalAsync(request.Email, request.Senha));
    }

    [HttpPost("esqueci-senha")]
    public async Task<IActionResult> EsqueciSenha([FromBody] EsqueciSenhaRequest request)
    {
        if (!Enum.TryParse<CanalRecuperacao>(request.Canal, ignoreCase: true, out var canal))
        {
            return BadRequest(new { mensagem = "Canal inválido." });
        }

        await _authService.SolicitarRecuperacaoAsync(request.Email, canal);
        // Resposta sempre genérica: não revela se o e-mail existe.
        return Ok(new { mensagem = "Se a conta existir, enviamos o código de recuperação." });
    }

    [HttpPost("verificar-codigo")]
    public async Task<ActionResult<VerificarCodigoResponse>> VerificarCodigo([FromBody] VerificarCodigoRequest request)
    {
        var resetToken = await _authService.VerificarCodigoAsync(request.Email, request.Codigo);
        return Ok(new VerificarCodigoResponse { ResetToken = resetToken });
    }

    [HttpPost("redefinir-senha")]
    public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaRequest request)
    {
        await _authService.RedefinirSenhaAsync(request.ResetToken, request.NovaSenha);
        return Ok(new { mensagem = "Senha redefinida com sucesso." });
    }

    [HttpPost("verificar-email")]
    public async Task<ActionResult<AuthResponse>> VerificarEmail([FromBody] VerificarCodigoRequest request)
    {
        return Ok(await _authService.VerificarEmailAsync(request.Email, request.Codigo));
    }

    [HttpPost("reenviar-verificacao")]
    public async Task<IActionResult> ReenviarVerificacao([FromBody] ReenviarVerificacaoRequest request)
    {
        await _authService.ReenviarVerificacaoAsync(request.Email);
        return Ok(new { mensagem = "Se a conta existir e não estiver verificada, reenviamos o código." });
    }
}
