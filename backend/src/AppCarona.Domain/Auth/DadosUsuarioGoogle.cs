namespace AppCarona.Domain.Auth;

/// <summary>Dados extraídos e validados de um id_token do Google.</summary>
public class DadosUsuarioGoogle
{
    public string Sub { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
}
