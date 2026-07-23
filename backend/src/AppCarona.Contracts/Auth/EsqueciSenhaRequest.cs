using System.ComponentModel.DataAnnotations;

namespace AppCarona.Contracts.Auth;

/// <summary>Solicita o envio do código de recuperação.</summary>
public class EsqueciSenhaRequest
{
    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [StringLength(200, ErrorMessage = "E-mail muito longo.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Canal de envio: "Email" ou "WhatsApp".</summary>
    [Required(ErrorMessage = "Informe o canal.")]
    public string Canal { get; set; } = "Email";
}
