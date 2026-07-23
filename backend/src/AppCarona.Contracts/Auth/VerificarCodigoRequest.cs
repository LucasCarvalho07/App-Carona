using System.ComponentModel.DataAnnotations;

namespace AppCarona.Contracts.Auth;

/// <summary>Verifica o código de recuperação digitado pelo usuário.</summary>
public class VerificarCodigoRequest
{
    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o código.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "O código deve ter 6 dígitos.")]
    public string Codigo { get; set; } = string.Empty;
}
