using AppCarona.Contracts.Motorista;

namespace AppCarona.Domain.Interfaces.Services;

public interface IMotoristaService
{
    /// <summary>Motoristas ativos, com indicação de quem já configurou o veículo.</summary>
    Task<IList<MotoristaOpcaoDto>> ListarOpcoesAsync();

    /// <summary>Detalhes de custo que o motorista repassa (veículo + config do mês).</summary>
    Task<DetalheMotoristaDto?> DetalhesAsync(int motoristaId, int anoMes);
}
