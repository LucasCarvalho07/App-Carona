using AppCarona.Domain.Entities;

namespace AppCarona.Domain.Interfaces.Repositories;

public interface IVeiculoRepository : IRepositoryBase<Veiculo>
{
    Task<IList<Veiculo>> ListarPorMotoristaAsync(int motoristaId);
    Task<Veiculo?> ObterPadraoAsync(int motoristaId);
}
