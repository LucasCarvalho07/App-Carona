using AppCarona.Contracts.Usuarios;
using AppCarona.Domain.Enums;
using AppCarona.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppCarona.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(Papel.Master))]
public class UsuariosController : AppControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public async Task<ActionResult<IList<UsuarioDto>>> Listar()
    {
        var usuarios = await _usuarioService.ListarAsync();
        return Ok(usuarios);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioDto>> ObterPorId(int id)
    {
        var usuario = await _usuarioService.ObterPorIdAsync(id);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpPut("{id:int}/aprovar")]
    public async Task<ActionResult<UsuarioDto>> Aprovar(int id, [FromBody] AprovarUsuarioRequest request)
    {
        var usuario = await _usuarioService.AprovarAsync(id, request.Papeis);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    /// <summary>Torna outro usuário master (ou remove). Só o master principal pode.</summary>
    [HttpPut("{id:int}/master")]
    public async Task<ActionResult<UsuarioDto>> DefinirMaster(int id, [FromBody] DefinirMasterRequest request)
    {
        var usuario = await _usuarioService.DefinirMasterAsync(id, request.TornarMaster, UsuarioLogadoEmail);
        return usuario is null ? NotFound() : Ok(usuario);
    }
}
