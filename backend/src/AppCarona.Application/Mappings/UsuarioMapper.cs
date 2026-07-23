using AppCarona.Contracts.Usuarios;
using AppCarona.Domain.Entities;

namespace AppCarona.Application.Mappings;

public static class UsuarioMapper
{
    public static UsuarioDto ParaDto(Usuario usuario)
    {
        return new UsuarioDto
        {
            Id = usuario.Id,
            Email = usuario.Email,
            Nome = usuario.Nome,
            Telefone = usuario.Telefone,
            FotoUrl = usuario.FotoUrl,
            Avatar = usuario.Avatar,
            Status = usuario.Status.ToString(),
            Papeis = usuario.Papeis.Select(p => p.Papel.ToString()).ToArray(),
            CriadoEm = usuario.CriadoEm,
            EmailVerificado = usuario.EmailVerificado
        };
    }

    /// <summary>Indica se o e-mail está na lista de masters principais (config).</summary>
    public static bool EhMasterPrincipal(string email, IEnumerable<string> masterEmails)
    {
        return masterEmails.Any(e => e.Equals(email, StringComparison.OrdinalIgnoreCase));
    }
}
