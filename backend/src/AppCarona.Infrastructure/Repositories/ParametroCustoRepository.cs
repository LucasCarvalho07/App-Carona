using AppCarona.Domain.Entities;
using AppCarona.Domain.Interfaces.Repositories;
using NHibernate;
using NHibernate.Linq;

namespace AppCarona.Infrastructure.Repositories;

public class ParametroCustoRepository : RepositoryBase<ParametroCusto>, IParametroCustoRepository
{
    public ParametroCustoRepository(ISession session) : base(session)
    {
    }

    public async Task<ParametroCusto?> ObterVigenteEmAsync(DateTime data)
    {
        var dia = data.Date;
        return await Session.Query<ParametroCusto>()
            .Where(p => p.VigenteDe <= dia)
            .OrderByDescending(p => p.VigenteDe)
            .FirstOrDefaultAsync();
    }

    public async Task<ParametroCusto?> ObterPorVigenteDeAsync(DateTime vigenteDe)
    {
        var dia = vigenteDe.Date;
        return await Session.Query<ParametroCusto>()
            .SingleOrDefaultAsync(p => p.VigenteDe == dia);
    }
}
