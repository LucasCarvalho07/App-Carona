using AppCarona.Domain.Entities;
using AppCarona.Domain.Enums;

namespace AppCarona.Domain.Interfaces.Repositories;

public interface IViagemRepository : IRepositoryBase<Viagem>
{
    Task<Viagem?> ObterPorDataMotoristaSentidoAsync(DateTime data, int motoristaId, Sentido sentido);
    Task<IList<Viagem>> ListarPorDataSentidoAsync(DateTime data, Sentido sentido);
    Task<IList<Viagem>> ListarPorParticipantePeriodoAsync(int passageiroId, DateTime inicio, DateTime fim);
    Task<IList<Viagem>> ListarPorMotoristaPeriodoAsync(int motoristaId, DateTime inicio, DateTime fim);
    Task<IList<Viagem>> ListarPorPeriodoAsync(DateTime inicio, DateTime fim);
}
