using AppCarona.Contracts.Usuarios;
using AppCarona.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppCarona.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PerfilController : AppControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public PerfilController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpPut("avatar")]
    public async Task<ActionResult<UsuarioDto>> AtualizarAvatar([FromBody] AtualizarAvatarRequest request)
    {
        var usuario = await _usuarioService.AtualizarAvatarAsync(UsuarioLogadoId, request.Avatar);
        return usuario is null ? NotFound() : Ok(usuario);
    }
}
