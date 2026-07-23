using AppCarona.Domain.Entities;
using AppCarona.Domain.Enums;
using AppCarona.Domain.Interfaces.Repositories;
using NHibernate;
using NHibernate.Linq;

namespace AppCarona.Infrastructure.Repositories;

public class ViagemRepository : RepositoryBase<Viagem>, IViagemRepository
{
    public ViagemRepository(ISession session) : base(session)
    {
    }

    public async Task<Viagem?> ObterPorDataMotoristaSentidoAsync(DateTime data, int motoristaId, Sentido sentido)
    {
        return await Session.Query<Viagem>()
            .SingleOrDefaultAsync(v => v.Data == data.Date
                && v.Motorista.Id == motoristaId
                && v.Sentido == sentido);
    }

    public async Task<IList<Viagem>> ListarPorDataSentidoAsync(DateTime data, Sentido sentido)
    {
        return await Session.Query<Viagem>()
            .Where(v => v.Data == data.Date && v.Sentido == sentido)
            .ToListAsync();
    }

    public async Task<IList<Viagem>> ListarPorParticipantePeriodoAsync(int passageiroId, DateTime inicio, DateTime fim)
    {
        return await Session.Query<Viagem>()
            .Where(v => v.Data >= inicio && v.Data < fim
                && v.Participacoes.Any(p => p.Passageiro.Id == passageiroId))
            .ToListAsync();
    }

    public async Task<IList<Viagem>> ListarPorMotoristaPeriodoAsync(int motoristaId, DateTime inicio, DateTime fim)
    {
        return await Session.Query<Viagem>()
            .Where(v => v.Data >= inicio && v.Data < fim && v.Motorista.Id == motoristaId)
            .ToListAsync();
    }

    public async Task<IList<Viagem>> ListarPorPeriodoAsync(DateTime inicio, DateTime fim)
    {
        return await Session.Query<Viagem>()
            .Where(v => v.Data >= inicio && v.Data < fim)
            .ToListAsync();
    }
}
