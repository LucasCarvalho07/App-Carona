using System.ComponentModel.DataAnnotations;

namespace AppCarona.Contracts.Auth;

/// <summary>Enviado pelo frontend após o login com Google (Google Identity Services).</summary>
public class LoginGoogleRequest
{
    [Required(ErrorMessage = "Token do Google ausente.")]
    public string IdToken { get; set; } = string.Empty;
}
