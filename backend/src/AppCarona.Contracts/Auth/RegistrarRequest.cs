using System.ComponentModel.DataAnnotations;

namespace AppCarona.Contracts.Auth;

/// <summary>Cadastro local (e-mail + senha).</summary>
public class RegistrarRequest
{
    [Required(ErrorMessage = "Informe o nome.")]
    [StringLength(120, ErrorMessage = "Nome muito longo.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [StringLength(200, ErrorMessage = "E-mail muito longo.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o telefone.")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "Telefone inválido.")]
    public string Telefone { get; set; } = string.Empty;

    // Regra de força (maiúscula + especial) é validada no serviço, com mensagem única.
    [Required(ErrorMessage = "Informe a senha.")]
    [StringLength(100, ErrorMessage = "Senha muito longa.")]
    public string Senha { get; set; } = string.Empty;
}
