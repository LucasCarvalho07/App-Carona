using AppCarona.Domain.Entities;
using AppCarona.Domain.Interfaces.Repositories;
using NHibernate;
using NHibernate.Linq;

namespace AppCarona.Infrastructure.Repositories;

public class PagamentoRepository : RepositoryBase<Pagamento>, IPagamentoRepository
{
    public PagamentoRepository(ISession session) : base(session)
    {
    }

    public async Task<Pagamento?> ObterAsync(int passageiroId, int motoristaId, int anoMes)
    {
        return await Session.Query<Pagamento>()
            .SingleOrDefaultAsync(p => p.Passageiro.Id == passageiroId
                && p.Motorista.Id == motoristaId
                && p.AnoMes == anoMes);
    }

    public async Task<IList<Pagamento>> ListarPorMotoristaMesAsync(int motoristaId, int anoMes)
    {
        return await Session.Query<Pagamento>()
            .Where(p => p.Motorista.Id == motoristaId && p.AnoMes == anoMes)
            .ToListAsync();
    }
}
