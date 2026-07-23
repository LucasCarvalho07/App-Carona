using AppCarona.Domain.Entities;
using AppCarona.Domain.Interfaces.Repositories;
using AppCarona.Domain.Interfaces.Services;

namespace AppCarona.Application.Services;

/// <summary>
/// Rateio da carona (regras do app):
///   custoCombustivel = km / consumo × precoLitro
///   custoTotal       = custoCombustivel + km × custoKmManutencao
///   valorPorPessoa   = arredondar2( custoTotal / (passageiros + 1) )   // motorista também paga
/// </summary>
public class CalculoService : ICalculoService
{
    private readonly IParametroCustoRepository _parametroCustoRepository;

    public CalculoService(IParametroCustoRepository parametroCustoRepository)
    {
        _parametroCustoRepository = parametroCustoRepository;
    }

    public async Task RecalcularAsync(Viagem viagem)
    {
        // Preço/manutenção vigentes na data da viagem (histórico por data).
        var parametro = await _parametroCustoRepository.ObterVigenteEmAsync(viagem.Data);

        var km = viagem.Veiculo?.KmPorViagem ?? 0m;
        var consumo = viagem.Veiculo?.ConsumoKmLitro ?? 0m;
        var precoLitro = parametro?.PrecoLitro ?? 0m;
        var custoKmManutencao = parametro?.CustoKmManutencao ?? 0m;

        viagem.SnapKmPorViagem = km;
        viagem.SnapConsumoKmLitro = consumo;
        viagem.SnapPrecoLitro = precoLitro;
        viagem.SnapCustoKmManutencao = custoKmManutencao;

        var custoCombustivel = consumo > 0 ? km / consumo * precoLitro : 0m;
        var custoTotal = custoCombustivel + km * custoKmManutencao;

        // Motorista também paga: divide entre passageiros + 1.
        var qtdPessoas = viagem.Participacoes.Count + 1;
        var valorPorPessoa = Arredondar(custoTotal / qtdPessoas);

        viagem.CustoCombustivel = Arredondar(custoCombustivel);
        viagem.CustoTotal = Arredondar(custoTotal);
        viagem.QtdPessoas = qtdPessoas;
        viagem.ValorPorPessoa = valorPorPessoa;

        foreach (var participacao in viagem.Participacoes)
        {
            participacao.ValorDevido = valorPorPessoa;
        }
    }

    private static decimal Arredondar(decimal valor)
    {
        return Math.Round(valor, 2, MidpointRounding.AwayFromZero);
    }
}
