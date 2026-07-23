using AppCarona.Domain.Entities;

namespace AppCarona.Domain.Interfaces.Auth;

public interface IJwtTokenGenerator
{
    /// <summary>Gera o JWT da aplicação com as claims do usuário (id, e-mail, status).</summary>
    string Gerar(Usuario usuario);

    /// <summary>
    /// Gera um token curto de uso único para redefinição de senha
    /// (emitido só após o código de recuperação ser validado).
    /// </summary>
    string GerarTokenReset(int usuarioId);

    /// <summary>Valida o token de reset e retorna o id do usuário, ou null se inválido/expirado.</summary>
    int? ValidarTokenReset(string token);
}
