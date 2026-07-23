using System.ComponentModel.DataAnnotations;

namespace AppCarona.Contracts.Auth;

/// <summary>Redefine a senha usando o token obtido na verificação do código.</summary>
public class RedefinirSenhaRequest
{
    [Required(ErrorMessage = "Token de redefinição ausente.")]
    public string ResetToken { get; set; } = string.Empty;

    // Regra de força (maiúscula + especial) é validada no serviço, com mensagem única.
    [Required(ErrorMessage = "Informe a nova senha.")]
    [StringLength(100, ErrorMessage = "Senha muito longa.")]
    public string NovaSenha { get; set; } = string.Empty;
}
