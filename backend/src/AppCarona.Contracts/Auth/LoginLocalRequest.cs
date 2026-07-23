using System.ComponentModel.DataAnnotations;

namespace AppCarona.Contracts.Auth;

/// <summary>Login local (e-mail + senha).</summary>
public class LoginLocalRequest
{
    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [StringLength(200, ErrorMessage = "E-mail muito longo.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha.")]
    [StringLength(100, ErrorMessage = "Senha muito longa.")]
    public string Senha { get; set; } = string.Empty;
}
