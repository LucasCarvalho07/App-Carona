using AppCarona.Domain.Entities;
using AppCarona.Domain.Interfaces.Repositories;
using NHibernate;
using NHibernate.Linq;

namespace AppCarona.Infrastructure.Repositories;

public class MotoristaConfigRepository : RepositoryBase<MotoristaConfig>, IMotoristaConfigRepository
{
    public MotoristaConfigRepository(ISession session) : base(session)
    {
    }

    public async Task<MotoristaConfig?> ObterPorMotoristaAsync(int motoristaId)
    {
        return await Session.Query<MotoristaConfig>()
            .SingleOrDefaultAsync(c => c.Motorista.Id == motoristaId);
    }
}
