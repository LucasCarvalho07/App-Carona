using AppCarona.Domain.Enums;

namespace AppCarona.Domain.Entities;

/// <summary>
/// Código de recuperação de senha (fluxo "esqueci minha senha").
/// O código nunca é guardado em claro: armazenamos apenas o hash.
/// </summary>
public class CodigoRecuperacao
{
    public virtual int Id { get; set; }

    /// <summary>Usuário dono do código.</summary>
    public virtual int UsuarioId { get; set; }

    /// <summary>Hash do código de 6 dígitos (mesmo hasher da senha).</summary>
    public virtual string CodigoHash { get; set; } = string.Empty;

    public virtual CanalRecuperacao Canal { get; set; }

    /// <summary>Momento em que o código deixa de valer.</summary>
    public virtual DateTime ExpiraEm { get; set; }

    /// <summary>Tentativas de digitação restantes (bloqueia após esgotar).</summary>
    public virtual int TentativasRestantes { get; set; }

    /// <summary>True depois de usado com sucesso (uso único).</summary>
    public virtual bool Usado { get; set; }

    public virtual DateTime CriadoEm { get; set; }
}
