using AppCarona.Domain.Entities;
using AppCarona.Domain.Interfaces.Repositories;
using NHibernate;
using NHibernate.Linq;

namespace AppCarona.Infrastructure.Repositories;

public class VeiculoRepository : RepositoryBase<Veiculo>, IVeiculoRepository
{
    public VeiculoRepository(ISession session) : base(session)
    {
    }

    public async Task<IList<Veiculo>> ListarPorMotoristaAsync(int motoristaId)
    {
        return await Session.Query<Veiculo>()
            .Where(v => v.Motorista.Id == motoristaId)
            .ToListAsync();
    }

    public async Task<Veiculo?> ObterPadraoAsync(int motoristaId)
    {
        return await Session.Query<Veiculo>()
            .Where(v => v.Motorista.Id == motoristaId && v.Padrao && v.Ativo)
            .FirstOrDefaultAsync();
    }
}
