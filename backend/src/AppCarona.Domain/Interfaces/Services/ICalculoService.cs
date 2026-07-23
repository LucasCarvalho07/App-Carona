using AppCarona.Domain.Entities;

namespace AppCarona.Domain.Interfaces.Services;

public interface ICalculoService
{
    /// <summary>
    /// Recalcula custo e rateio de uma viagem, gravando snapshot dos parâmetros
    /// e o valor devido por participante. Não persiste (quem chama salva).
    /// </summary>
    Task RecalcularAsync(Viagem viagem);
}
