using AppCarona.Contracts.Motorista;
using AppCarona.Domain.Entities;
using AppCarona.Domain.Interfaces.Repositories;
using AppCarona.Domain.Interfaces.Services;

namespace AppCarona.Application.Services;

public class MotoristaService : IMotoristaService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IParametroCustoRepository _parametroCustoRepository;

    public MotoristaService(
        IUsuarioRepository usuarioRepository,
        IVeiculoRepository veiculoRepository,
        IParametroCustoRepository parametroCustoRepository)
    {
        _usuarioRepository = usuarioRepository;
        _veiculoRepository = veiculoRepository;
        _parametroCustoRepository = parametroCustoRepository;
    }

    public async Task<IList<MotoristaOpcaoDto>> ListarOpcoesAsync()
    {
        var motoristas = await _usuarioRepository.ListarMotoristasAtivosAsync();
        var opcoes = new List<MotoristaOpcaoDto>();

        foreach (var motorista in motoristas)
        {
            var veiculo = await _veiculoRepository.ObterPadraoAsync(motorista.Id);
            opcoes.Add(new MotoristaOpcaoDto
            {
                Id = motorista.Id,
                Nome = motorista.Nome,
                Avatar = motorista.Avatar,
                Configurado = VeiculoConfigurado(veiculo),
            });
        }

        return opcoes.OrderBy(o => o.Nome).ToList();
    }

    public async Task<DetalheMotoristaDto?> DetalhesAsync(int motoristaId, int anoMes)
    {
        var motorista = await _usuarioRepository.ObterPorIdAsync(motoristaId);
        if (motorista is null)
        {
            return null;
        }

        var veiculo = await _veiculoRepository.ObterPadraoAsync(motoristaId);
        // Parâmetro vigente no último dia do mês de referência.
        var ano = anoMes / 100;
        var mes = anoMes % 100;
        var ultimoDiaDoMes = new DateTime(ano, mes, 1).AddMonths(1).AddDays(-1);
        var config = await _parametroCustoRepository.ObterVigenteEmAsync(ultimoDiaDoMes);

        var km = veiculo?.KmPorViagem ?? 0m;
        var consumo = veiculo?.ConsumoKmLitro ?? 0m;
        var precoLitro = config?.PrecoLitro ?? 0m;
        var manutencao = config?.CustoKmManutencao ?? 0m;

        var custoCombustivel = consumo > 0 ? km / consumo * precoLitro : 0m;
        var custoTotal = custoCombustivel + km * manutencao;

        return new DetalheMotoristaDto
        {
            MotoristaId = motoristaId,
            MotoristaNome = motorista.Nome,
            AnoMes = anoMes,
            Configurado = VeiculoConfigurado(veiculo),
            TemConfigMes = config is not null,
            VeiculoNome = veiculo?.Nome,
            ConsumoKmLitro = consumo,
            KmPorViagem = km,
            PrecoLitro = precoLitro,
            CustoKmManutencao = manutencao,
            CustoCombustivel = decimal.Round(custoCombustivel, 2),
            CustoTotal = decimal.Round(custoTotal, 2),
        };
    }

    private static bool VeiculoConfigurado(Veiculo? veiculo)
    {
        return veiculo is not null && veiculo.ConsumoKmLitro > 0 && veiculo.KmPorViagem > 0;
    }
}
