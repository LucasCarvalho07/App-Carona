using AppCarona.Contracts.Motorista;

namespace AppCarona.Domain.Interfaces.Services;

public interface IMotoristaConfigService
{
    Task<MotoristaConfigDto?> ObterAsync(int motoristaId);
    Task<MotoristaConfigDto> SalvarAsync(int motoristaId, SalvarMotoristaConfigRequest request);
}
