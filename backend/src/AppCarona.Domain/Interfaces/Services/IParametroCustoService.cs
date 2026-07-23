using AppCarona.Contracts.Custo;

namespace AppCarona.Domain.Interfaces.Services;

public interface IParametroCustoService
{
    Task<IList<ParametroCustoDto>> ListarAsync();
    Task<ParametroCustoDto> SalvarAsync(SalvarParametroCustoRequest request);
}
