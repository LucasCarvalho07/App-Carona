using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace AppCarona.Api.Controllers;

public abstract class AppControllerBase : ControllerBase
{
    /// <summary>Id do usuário autenticado (claim sub do JWT).</summary>
    protected int UsuarioLogadoId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : throw new UnauthorizedAccessException("Token sem identificador de usuário válido.");

    /// <summary>E-mail do usuário autenticado (claim do JWT).</summary>
    protected string UsuarioLogadoEmail =>
        User.FindFirstValue(ClaimTypes.Email)
            ?? throw new UnauthorizedAccessException("Token sem e-mail do usuário.");
}
