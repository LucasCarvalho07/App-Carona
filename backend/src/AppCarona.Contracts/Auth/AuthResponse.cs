using AppCarona.Contracts.Usuarios;

namespace AppCarona.Contracts.Auth;

/// <summary>Resposta do login: JWT da aplicação + dados do usuário.</summary>
public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public UsuarioDto Usuario { get; set; } = new();
}
