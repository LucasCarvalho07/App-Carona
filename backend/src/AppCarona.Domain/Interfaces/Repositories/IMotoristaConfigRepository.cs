using AppCarona.Domain.Entities;

namespace AppCarona.Domain.Interfaces.Repositories;

public interface IMotoristaConfigRepository : IRepositoryBase<MotoristaConfig>
{
    Task<MotoristaConfig?> ObterPorMotoristaAsync(int motoristaId);
}
