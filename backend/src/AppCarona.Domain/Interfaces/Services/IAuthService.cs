using AppCarona.Contracts.Auth;
using AppCarona.Domain.Enums;

namespace AppCarona.Domain.Interfaces.Services;

public interface IAuthService
{
    /// <summary>
    /// Valida o id_token do Google, cria o usuário no primeiro acesso
    /// (status AguardandoAprovacao) e devolve o JWT da aplicação.
    /// </summary>
    Task<AuthResponse> LoginComGoogleAsync(string idToken);

    /// <summary>Cadastro local: cria o usuário com senha e devolve o JWT.</summary>
    Task<AuthResponse> RegistrarLocalAsync(string nome, string email, string telefone, string senha);

    /// <summary>Login local: valida e-mail/senha e devolve o JWT.</summary>
    Task<AuthResponse> LoginLocalAsync(string email, string senha);

    /// <summary>
    /// Gera e envia um código de recuperação pelo canal escolhido.
    /// Não revela se a conta existe (resposta sempre genérica no controller).
    /// </summary>
    Task SolicitarRecuperacaoAsync(string email, CanalRecuperacao canal);

    /// <summary>Valida o código digitado e devolve um token de uso único para redefinir a senha.</summary>
    Task<string> VerificarCodigoAsync(string email, string codigo);

    /// <summary>Redefine a senha a partir do token de reset (uso único).</summary>
    Task RedefinirSenhaAsync(string resetToken, string novaSenha);

    /// <summary>Confirma a posse do e-mail via código; promove a master se for o principal.</summary>
    Task<AuthResponse> VerificarEmailAsync(string email, string codigo);

    /// <summary>Reenvia o código de verificação de e-mail.</summary>
    Task ReenviarVerificacaoAsync(string email);
}
