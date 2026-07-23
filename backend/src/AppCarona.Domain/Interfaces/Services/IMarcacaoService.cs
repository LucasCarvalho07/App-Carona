using AppCarona.Contracts.Marcacao;
using AppCarona.Domain.Enums;

namespace AppCarona.Domain.Interfaces.Services;

public interface IMarcacaoService
{
    /// <summary>Marca presença do passageiro numa carona (ida ou volta); cria/acha a viagem e recalcula.</summary>
    Task<MinhaMarcacaoDto> MarcarAsync(int passageiroId, DateTime data, int motoristaId, Sentido sentido);

    /// <summary>Remove a presença do passageiro na viagem daquele dia/motorista/sentido (recalcula).</summary>
    Task DesmarcarAsync(int passageiroId, DateTime data, int motoristaId, Sentido sentido);

    /// <summary>Marcações do passageiro no mês (AAAAMM).</summary>
    Task<IList<MinhaMarcacaoDto>> ListarMinhasAsync(int passageiroId, int anoMes);
}
