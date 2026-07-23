using AppCarona.Domain.Interfaces.Repositories;
using NHibernate;
using NHibernate.Linq;

namespace AppCarona.Infrastructure.Repositories;

/// <summary>
/// Implementação genérica sobre a ISession do NHibernate. Cada escrita roda em
/// sua própria transação para garantir consistência.
/// </summary>
public class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected readonly ISession Session;

    public RepositoryBase(ISession session)
    {
        Session = session;
    }

    public virtual async Task<T?> ObterPorIdAsync(int id)
    {
        return await Session.GetAsync<T>(id);
    }

    public virtual async Task<IList<T>> ListarAsync()
    {
        return await Session.Query<T>().ToListAsync();
    }

    public virtual async Task SalvarAsync(T entidade)
    {
        using var transacao = Session.BeginTransaction();
        await Session.SaveAsync(entidade);
        await transacao.CommitAsync();
    }

    public virtual async Task AtualizarAsync(T entidade)
    {
        using var transacao = Session.BeginTransaction();
        await Session.UpdateAsync(entidade);
        await transacao.CommitAsync();
    }

    public virtual async Task RemoverAsync(T entidade)
    {
        using var transacao = Session.BeginTransaction();
        await Session.DeleteAsync(entidade);
        await transacao.CommitAsync();
    }
}
