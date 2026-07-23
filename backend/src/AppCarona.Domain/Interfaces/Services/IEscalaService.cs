using AppCarona.Contracts.Escala;
using AppCarona.Domain.Enums;

namespace AppCarona.Domain.Interfaces.Services;

public interface IEscalaService
{
    /// <summary>Motorista se escala num trecho (cria a viagem vazia). Valida veículo e conflito.</summary>
    Task EscalarAsync(int motoristaId, DateTime data, Sentido sentido);

    /// <summary>Escala a semana inteira (seg–sex, ida e volta) do motorista; ignora dias em conflito.</summary>
    Task EscalarSemanaAsync(int motoristaId, DateTime dataNaSemana);

    /// <summary>Remove a escala do motorista no trecho (só se não houver passageiros).</summary>
    Task DesescalarAsync(int motoristaId, DateTime data, Sentido sentido);

    /// <summary>Todos os carros escalados no mês (AAAAMM), com flags do usuário logado.</summary>
    Task<IList<EscalaCarroDto>> ListarEscalaMesAsync(int usuarioLogadoId, int anoMes);
}
