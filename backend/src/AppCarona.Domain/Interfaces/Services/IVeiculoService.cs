using AppCarona.Contracts.Veiculos;

namespace AppCarona.Domain.Interfaces.Services;

public interface IVeiculoService
{
    Task<IList<VeiculoDto>> ListarDoMotoristaAsync(int motoristaId);
    Task<VeiculoDto> CriarAsync(int motoristaId, SalvarVeiculoRequest request);
    Task<VeiculoDto?> AtualizarAsync(int motoristaId, int id, SalvarVeiculoRequest request);
}
