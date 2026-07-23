using AppCarona.Domain.Auth;

namespace AppCarona.Domain.Interfaces.Auth;

public interface IGoogleTokenValidator
{
    /// <summary>Valida o id_token junto ao Google. Lança exceção se inválido.</summary>
    Task<DadosUsuarioGoogle> ValidarAsync(string idToken);
}
