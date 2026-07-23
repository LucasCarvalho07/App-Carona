using AppCarona.Contracts.Pagamentos;

namespace AppCarona.Domain.Interfaces.Services;

public interface IPagamentoService
{
    /// <summary>Quanto o passageiro deve a cada motorista no mês (com status).</summary>
    Task<IList<PagamentoResumoDto>> ResumoDoPassageiroAsync(int passageiroId, int anoMes);

    /// <summary>Passageiro informa que pagou um motorista no mês.</summary>
    Task InformarAsync(int passageiroId, int motoristaId, int anoMes);

    /// <summary>Lista os pagadores de um motorista no mês (falta pagar / pagaram).</summary>
    Task<IList<RecebimentoDto>> RecebimentosAsync(int motoristaId, int anoMes);

    /// <summary>Resumo do mês por motorista (todos): passageiros, dias e valores.</summary>
    Task<IList<ResumoMensalMotoristaDto>> ResumoMensalAsync(int anoMes);

    /// <summary>Motorista confirma o recebimento de um pagamento.</summary>
    Task ConfirmarAsync(int motoristaId, int pagamentoId);

    /// <summary>Motorista rejeita um pagamento informado.</summary>
    Task RejeitarAsync(int motoristaId, int pagamentoId);
}
