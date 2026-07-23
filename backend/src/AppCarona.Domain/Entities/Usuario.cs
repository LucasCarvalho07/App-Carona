using AppCarona.Domain.Enums;

namespace AppCarona.Domain.Entities;

/// <summary>
/// Usuário do sistema. Propriedades virtuais são exigidas pelo NHibernate (lazy loading/proxy).
/// </summary>
public class Usuario
{
    public virtual int Id { get; set; }

    /// <summary>Identificador único da conta Google (claim 'sub'). Nulo para contas locais.</summary>
    public virtual string? GoogleSub { get; set; }

    public virtual string Email { get; set; } = string.Empty;

    public virtual string Nome { get; set; } = string.Empty;

    /// <summary>Telefone de contato (obrigatório no cadastro local; nulo em contas antigas/Google).</summary>
    public virtual string? Telefone { get; set; }

    public virtual string? FotoUrl { get; set; }

    /// <summary>Avatar escolhido pelo usuário (seed do gerador). Nulo = usa foto/iniciais.</summary>
    public virtual string? Avatar { get; set; }

    /// <summary>Hash da senha (login local). Nulo para contas que entram só via Google.</summary>
    public virtual string? SenhaHash { get; set; }

    /// <summary>True quando o dono comprovou posse do e-mail (Google = sempre; local = via código).</summary>
    public virtual bool EmailVerificado { get; set; }

    public virtual StatusUsuario Status { get; set; } = StatusUsuario.AguardandoAprovacao;

    public virtual DateTime CriadoEm { get; set; }

    public virtual IList<UsuarioPapel> Papeis { get; set; } = new List<UsuarioPapel>();

    public virtual bool PossuiPapel(Papel papel)
    {
        return Papeis.Any(p => p.Papel == papel);
    }

    /// <summary>Adiciona o papel se ainda não existir. Retorna true se adicionou.</summary>
    public virtual bool GarantirPapel(Papel papel)
    {
        if (PossuiPapel(papel))
        {
            return false;
        }
        Papeis.Add(new UsuarioPapel { Papel = papel });
        return true;
    }
}
