using AppCarona.Domain.Entities;

namespace AppCarona.Domain.Interfaces.Repositories;

public interface IPagamentoRepository : IRepositoryBase<Pagamento>
{
    Task<Pagamento?> ObterAsync(int passageiroId, int motoristaId, int anoMes);
    Task<IList<Pagamento>> ListarPorMotoristaMesAsync(int motoristaId, int anoMes);
}
